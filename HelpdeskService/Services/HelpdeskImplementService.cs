////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using Newtonsoft.Json;
using SharedLib;
using DbcLib;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace HelpdeskService;

/// <summary>
/// Helpdesk - Implement
/// </summary>
public class HelpdeskImplementService(
    ILogger<HelpdeskImplementService> loggerRepo,
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IManualCustomCacheService cacheRepo,
    IOptions<HelpdeskConfigModel> hdConf,
    ICommerceRemoteTransmissionService commRepo,
    IHttpClientFactory HttpClientFactory,
    IMemoryCache cache,
    ITelegramRemoteTransmissionService telegramRemoteRepo,
    ISerializeStorageRemoteTransmissionService StorageRepo,
    IWebRemoteTransmissionService webTransmissionRepo) : IHelpdeskService
{
    static readonly CultureInfo cultureInfo = new("ru-RU");
    static readonly TimeSpan _ts = TimeSpan.FromSeconds(5);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<IssueHelpdeskModel>> ConsoleIssuesSelect(TPaginationRequestModel<ConsoleIssuesRequestModel> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        string ConsoleSegmentNewCacheToken(StatusesDocumentsEnum st) => $"{GlobalStaticConstants.Routes.CONSOLE_CONTROLLER_NAME}:{GlobalStaticConstants.Routes.SEGMENT_CONTROLLER_NAME}:{st}:{req.Payload.ProjectId}:{req.PageSize}:{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.{Guid.NewGuid()}";
        string? cacheToken = null;

        if (req.PageNum == 0 && string.IsNullOrWhiteSpace(req.Payload.SearchQuery) && string.IsNullOrWhiteSpace(req.Payload.FilterUserId))
        {
            MemCacheComplexKeyModel mceKey = GlobalStaticConstants.Cache.ConsoleSegmentStatusToken(req.Payload.Status);
            cacheToken = await cacheRepo.GetStringValueAsync(mceKey);
            if (string.IsNullOrWhiteSpace(cacheToken))
            {
                cacheToken = ConsoleSegmentNewCacheToken(req.Payload.Status);
                await cacheRepo.SetStringAsync(mceKey, cacheToken);
            }
        }

        if (!string.IsNullOrWhiteSpace(cacheToken))
        {
            TPaginationResponseModel<IssueHelpdeskModel>? _fr = await cacheRepo.GetObjectAsync<TPaginationResponseModel<IssueHelpdeskModel>>(cacheToken);
            if (_fr is not null)
                return _fr;
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IQueryable<IssueHelpdeskModelDB> q = context
            .Issues
            .Where(x => x.ProjectId == req.Payload.ProjectId && x.StepIssue == req.Payload.Status)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
        {
            req.Payload.SearchQuery = req.Payload.SearchQuery.ToUpper();
            q = from issue_element in q
                join rubric_element in context.Rubrics on issue_element.RubricIssueId equals rubric_element.Id into outer_rubric
                from rubric_item in outer_rubric.DefaultIfEmpty()
                where issue_element.NormalizedNameUpper!.Contains(req.Payload.SearchQuery) || issue_element.NormalizedDescriptionUpper!.Contains(req.Payload.SearchQuery) || rubric_item.NormalizedNameUpper!.Contains(req.Payload.SearchQuery)
                select issue_element;
        }

        if (!string.IsNullOrWhiteSpace(req.Payload.FilterUserId))
            q = q.Where(x => x.AuthorIdentityUserId == req.Payload.FilterUserId || x.ExecutorIdentityUserId == req.Payload.FilterUserId || context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && y.UserId == req.Payload.FilterUserId));

        IOrderedQueryable<IssueHelpdeskModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
            ? q.OrderBy(x => x.CreatedAt)
            : q.OrderByDescending(x => x.CreatedAt);

        List<IssueHelpdeskModelDB> data = await oq
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .Include(x => x.RubricIssue)
            .ToListAsync();

        TPaginationResponseModel<IssueHelpdeskModel> res = new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortingDirection = req.SortingDirection,
            SortBy = req.SortBy,
            TotalRowsCount = await q.CountAsync(),
            Response = [.. data.Select(x => IssueHelpdeskModel.Build(x))]
        };

        if (!string.IsNullOrWhiteSpace(cacheToken))
            await cacheRepo.SetObject(cacheToken, res, TimeSpan.FromSeconds(hdConf.Value.ConsoleSegmentCacheLifetimeSeconds));

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ExecuterUpdate(TAuthRequestModel<UserIssueModel> req)
    {
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<bool> res = new();

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = req.SenderActionUserId,
            Payload = new IssuesReadRequestModel()
            {
                IssuesIds = [req.Payload.IssueId],
                IncludeSubscribersOnly = true,
            },
        });

        if (!issues_data.Success() || issues_data.Response is null || issues_data.Response.Length == 0)
            return new() { Messages = issues_data.Messages };

        IssueHelpdeskModelDB issue_data = issues_data.Response.Single();

        string[] users_ids = [req.SenderActionUserId, req.Payload.UserId, issue_data.ExecutorIdentityUserId ?? ""];
        users_ids = [.. users_ids.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()];

        TResponseModel<UserInfoModel[]?> users_rest = await webTransmissionRepo.GetUsersIdentity(users_ids);
        if (!users_rest.Success() || users_rest.Response is null || users_rest.Response.Length != users_ids.Length)
            return new() { Messages = users_rest.Messages };

        UserInfoModel actor = users_rest.Response.First(x => x.UserId == req.SenderActionUserId);
        UserInfoModel? requested_user = users_rest.Response.FirstOrDefault(x => x.UserId == req.Payload.UserId);

        if (req.SenderActionUserId != GlobalStaticConstants.Roles.System && issue_data.Subscribers?.Any(x => x.UserId == req.SenderActionUserId) != true)
        {
            await SubscribeUpdate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    IssueId = issue_data.Id,
                    SetValue = true,
                    UserId = actor.UserId,
                    IsSilent = false,
                }
            });
        }
        if (issue_data.Subscribers?.Any(x => x.UserId == req.Payload.UserId) != true)
        {
            await SubscribeUpdate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    IssueId = issue_data.Id,
                    SetValue = true,
                    UserId = req.Payload.UserId,
                    IsSilent = false,
                }
            });
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        string msg;
        if (string.IsNullOrWhiteSpace(req.Payload.UserId))
        {
            if (string.IsNullOrWhiteSpace(issue_data.ExecutorIdentityUserId))
                res.AddInfo("Исполнитель уже отсутствует");
            else
            {
                if (issue_data.StepIssue >= StatusesDocumentsEnum.Progress)
                {
                    res.AddError($"Обращение в статусе [{issue_data.StepIssue.DescriptionInfo()}]. После того как обращение переходит в работу (и далее) удалить исполнителя нельзя. Для открепления исполнителя поставьте обращение на паузу");
                    return res;
                }

                await context
                    .Issues
                    .Where(x => x.Id == req.Payload.IssueId)
                    .ExecuteUpdateAsync(set => set.SetProperty(b => b.ExecutorIdentityUserId, req.Payload.UserId));

                msg = $"Исполнитель `{users_rest.Response.First(x => x.UserId == issue_data.ExecutorIdentityUserId).UserName}` успешно откреплён от обращения";
                res.AddSuccess(msg);

                PulseRequestModel p_req = new()
                {
                    Payload = new()
                    {
                        Payload = new()
                        {
                            IssueId = issue_data.Id,
                            PulseType = PulseIssuesTypesEnum.Executor,
                            Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                            Description = msg,
                        },
                        SenderActionUserId = req.SenderActionUserId
                    }
                };

                await PulsePush(p_req);
            }
        }
        else
        {
            if (issue_data.ExecutorIdentityUserId == req.Payload.UserId)
                res.AddInfo($"Исполнитель `{requested_user!.UserName}` уже установлен");
            else
            {
                // msg = $"Исполнитель обращения успешно установлен: {requested_user!.UserName}";
                msg = "Исполнитель обращения успешно";
                PulseRequestModel p_req;
                if (string.IsNullOrWhiteSpace(issue_data.ExecutorIdentityUserId))
                {
                    msg += $": установлен `{requested_user?.UserName}`";
                    p_req = new()
                    {
                        Payload = new()
                        {
                            Payload = new()
                            {
                                IssueId = issue_data.Id,
                                PulseType = PulseIssuesTypesEnum.Executor,
                                Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                                Description = msg,
                            },
                            SenderActionUserId = req.SenderActionUserId
                        }
                    };

                    await PulsePush(p_req);
                }
                else
                {
                    msg += $": изменён `{users_rest.Response.FirstOrDefault(x => x.UserId == issue_data.ExecutorIdentityUserId)}` в `{requested_user?.UserName}`";

                    p_req = new()
                    {
                        Payload = new()
                        {
                            Payload = new()
                            {
                                IssueId = issue_data.Id,
                                PulseType = PulseIssuesTypesEnum.Executor,
                                Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                                Description = msg,
                            },
                            SenderActionUserId = req.SenderActionUserId
                        }
                    };

                    await PulsePush(p_req);
                }

                await context
                    .Issues
                    .Where(x => x.Id == req.Payload.IssueId)
                    .ExecuteUpdateAsync(set => set.SetProperty(b => b.ExecutorIdentityUserId, req.Payload.UserId));

                res.AddSuccess(msg);
            }
        }
        await ConsoleSegmentCacheEmpty(issue_data.StepIssue);
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> IssueCreateOrUpdate(TAuthRequestModel<IssueUpdateRequestModel> issue_upd)
    {
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(issue_upd)}");
        TResponseModel<int> res = new() { Response = 0 };

        TResponseModel<UserInfoModel[]?> users_rest = await webTransmissionRepo.GetUsersIdentity([issue_upd.SenderActionUserId]);
        if (!users_rest.Success() || users_rest.Response is null || users_rest.Response.Length != 1)
            return new() { Messages = users_rest.Messages };

        UserInfoModel actor = users_rest.Response.First(x => x.UserId == issue_upd.SenderActionUserId);

        issue_upd.Payload.Description = issue_upd.Payload.Description?.Trim();
        string? normalizedDescriptionUpper = issue_upd.Payload.Description?.ToUpper();

        Regex rx = new(@"\s+", RegexOptions.Compiled);
        issue_upd.Payload.Name = rx.Replace(issue_upd.Payload.Name.Trim(), " ");

        string normalizedNameUpper = issue_upd.Payload.Name.ToUpper();

        IssueHelpdeskModelDB issue;
        DateTime dtn = DateTime.UtcNow;
        string msg;
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        PulseRequestModel p_req;
        TResponseModel<ModesSelectRubricsEnum?> res_ModeSelectingRubrics = await StorageRepo.ReadParameter<ModesSelectRubricsEnum?>(GlobalStaticConstants.CloudStorageMetadata.ModeSelectingRubrics);
        ModesSelectRubricsEnum _current_mode_rubric = res_ModeSelectingRubrics.Response ?? ModesSelectRubricsEnum.AllowWithoutRubric;
        if (_current_mode_rubric != ModesSelectRubricsEnum.AllowWithoutRubric)
        {
            string[] sub_rubrics = await context
                        .Rubrics
                        .Where(x => x.ParentRubricId == issue_upd.Payload.RubricId)
                        .Select(x => x.Name)
                        .ToArrayAsync();

            if (_current_mode_rubric != ModesSelectRubricsEnum.SelectAny && sub_rubrics.Length != 0)
            {
                res.AddError($"Требуется выбрать все подрубрики: {string.Join(",", sub_rubrics)};");
                return res;
            }
        }

        issue_upd.Payload.Description = issue_upd.Payload.Description?.Replace("\n", "<br/>");

        if (issue_upd.Payload.Id < 1)
        {
            issue = new()
            {
                AuthorIdentityUserId = issue_upd.SenderActionUserId,
                Name = issue_upd.Payload.Name.Trim(),
                Description = issue_upd.Payload.Description,
                RubricIssueId = issue_upd.Payload.RubricId,
                NormalizedNameUpper = normalizedNameUpper,
                NormalizedDescriptionUpper = normalizedDescriptionUpper,
                StepIssue = StatusesDocumentsEnum.Created,
                ProjectId = issue_upd.Payload.ProjectId,
                CreatedAt = dtn,
                LastUpdateAt = dtn,
            };

            IssueReadMarkerHelpdeskModelDB my_mark = new() { Issue = issue, LastReadAt = dtn, UserIdentityId = issue_upd.SenderActionUserId };
            issue.ReadMarkers = [my_mark];

            SubscriberIssueHelpdeskModelDB my_subscr = new() { Issue = issue, UserId = issue_upd.SenderActionUserId };
            issue.Subscribers = [my_subscr];

            try
            {
                await context.AddAsync(issue);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                loggerRepo.LogError(ex, $"Не удалось создать заявку для заказа");
                res.Messages.InjectException(ex);
                return res;
            }

            msg = "Обращение успешно создано";
            res.AddSuccess(msg);
            res.Response = issue.Id;
            p_req = new()
            {
                Payload = new()
                {
                    Payload = new()
                    {
                        IssueId = issue.Id,
                        PulseType = PulseIssuesTypesEnum.Status,
                        Tag = issue.StepIssue.DescriptionInfo(),
                        Description = msg,
                    },
                    SenderActionUserId = GlobalStaticConstants.Roles.System,
                },
                IsMuteEmail = true,
                IsMuteTelegram = true,
                IsMuteWhatsapp = true,
            };

            await PulsePush(p_req);

            await MessageUpdateOrCreate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    MessageText = $"Пользователь `{actor.UserName}` создал новый запрос: {issue_upd.Payload.Name}",
                    IssueId = issue.Id
                }
            });

            TResponseModel<long?> helpdesk_user_redirect_telegram_for_issue_rest = await StorageRepo.ReadParameter<long?>(GlobalStaticConstants.CloudStorageMetadata.HelpdeskNotificationTelegramForCreateIssue);
            if (helpdesk_user_redirect_telegram_for_issue_rest.Success() && helpdesk_user_redirect_telegram_for_issue_rest.Response.HasValue && helpdesk_user_redirect_telegram_for_issue_rest.Response != 0)
            {
                await telegramRemoteRepo.SendTextMessageTelegram(new()
                {
                    Message = $"Создана новая заявка: #{issue.Id} '{issue.Name}'. Автор: {actor}",
                    From = "уведомление",
                    UserTelegramId = helpdesk_user_redirect_telegram_for_issue_rest.Response.Value,
                });
            }
        }
        else
        {
            issue = await context
                .Issues
                .Include(x => x.Subscribers)
                .FirstAsync(x => x.Id == issue_upd.Payload.Id);

            if (issue.AuthorIdentityUserId == issue_upd.SenderActionUserId ||
                issue.ExecutorIdentityUserId == issue_upd.SenderActionUserId ||
                issue.Subscribers?.Any(x => x.UserId == issue_upd.SenderActionUserId) == true ||
                actor.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Contains(x)) == true ||
                actor.IsAdmin)
            {
                res.Response = await context.Issues.Where(x => x.Id == issue_upd.Payload.Id)
                                .ExecuteUpdateAsync(setters => setters
                                .SetProperty(b => b.Description, issue_upd.Payload.Description)
                                .SetProperty(b => b.NormalizedNameUpper, normalizedNameUpper)
                                .SetProperty(b => b.NormalizedDescriptionUpper, normalizedDescriptionUpper)
                                .SetProperty(b => b.RubricIssueId, issue_upd.Payload.RubricId)
                                .SetProperty(b => b.Name, issue_upd.Payload.Name)
                                .SetProperty(b => b.LastUpdateAt, DateTime.UtcNow));

                OrdersByIssuesSelectRequestModel req_comm = new()
                {
                    IncludeExternalData = true,
                    IssueIds = [issue_upd.Payload.Id],
                };

                TResponseModel<OrderDocumentModelDB[]> comm_res = await commRepo.OrdersByIssues(req_comm);
                TResponseModel<TelegramBotConfigModel?> wc = await webTransmissionRepo.GetWebConfig();
                msg = $"Документ (#{issue_upd.Payload.Id}) обновлён.";
                if (comm_res.Success() && comm_res.Response is not null && comm_res.Response.Length != 0)
                {
                    msg += $". Заказ: [{string.Join(";", comm_res.Response.Select(x => $"(№{x.Id} - {x.CreatedAtUTC.GetCustomTime()})"))}]";
                }
                msg += $". /<a href='{wc.Response?.ClearBaseUri}'>{wc.Response?.ClearBaseUri}</a>/";
                res.AddSuccess(msg);

                p_req = new()
                {
                    Payload = new()
                    {
                        Payload = new()
                        {
                            IssueId = issue.Id,
                            PulseType = PulseIssuesTypesEnum.Main,
                            Tag = GlobalStaticConstants.Routes.UPDATE_ACTION_NAME,
                            Description = msg,
                        },
                        SenderActionUserId = issue_upd.SenderActionUserId,
                    }
                };

                await PulsePush(p_req);

                if (issue_upd.SenderActionUserId != GlobalStaticConstants.Roles.System && issue.Subscribers?.Any(x => x.UserId == issue_upd.SenderActionUserId) != true)
                {
                    await SubscribeUpdate(new()
                    {
                        SenderActionUserId = GlobalStaticConstants.Roles.System,
                        Payload = new()
                        {
                            IssueId = issue.Id,
                            SetValue = true,
                            UserId = actor.UserId,
                            IsSilent = false,
                        }
                    });
                }
            }
            else
                res.AddError($"У вас не достаточно прав для редактирования этого обращения #{issue_upd.Payload.Id} '{issue.Name}'");
        }
        await ConsoleSegmentCacheEmpty(issue.StepIssue);
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB[]>> IssuesRead(TAuthRequestModel<IssuesReadRequestModel> req)
    {
        TResponseModel<IssueHelpdeskModelDB[]> res = new();
        string mem_key = $"{GlobalStaticConstants.TransmissionQueues.IssuesGetHelpdeskReceive}-{string.Join(";", req.Payload.IssuesIds)}/{req.Payload.IncludeSubscribersOnly}({req.SenderActionUserId})";
        if (cache.TryGetValue(mem_key, out IssueHelpdeskModelDB[]? hd))
        {
            if (hd is null)
                cache.Remove(mem_key);
            else
            {
                res.Response = hd;
                return res;
            }
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IIncludableQueryable<IssueHelpdeskModelDB, List<SubscriberIssueHelpdeskModelDB>?> q = context.Issues.Where(x => req.Payload.IssuesIds.Any(y => y == x.Id)).Include(x => x.Subscribers);
        IssueHelpdeskModelDB[]? issues_db = req.Payload.IncludeSubscribersOnly
            ? await q.ToArrayAsync()
            : await q.Include(x => x.RubricIssue).Include(x => x.Messages).ToArrayAsync();

        if (issues_db is null || issues_db.Length == 0)
        {
            loggerRepo.LogError($"Обращение не найдено: {mem_key}");
            return new()
            {
                Messages = [new() { TypeMessage = ResultTypesEnum.Warning, Text = "Обращение не найдено или у вас нет к нему доступа" }]
            };
        }

        if (req.SenderActionUserId == GlobalStaticConstants.Roles.System || issues_db.All(x => x.ExecutorIdentityUserId == req.SenderActionUserId) || issues_db.All(x => x.AuthorIdentityUserId == req.SenderActionUserId) || issues_db.All(x => x.Subscribers!.Any(x => x.UserId == req.SenderActionUserId)))
            return new() { Response = issues_db };

        TResponseModel<UserInfoModel[]?> rest_user_date = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!rest_user_date.Success() || rest_user_date.Response is null || rest_user_date.Response.Length != 1)
        {
            loggerRepo.LogError($"Пользователь не найден: {req.SenderActionUserId}");
            return new() { Messages = rest_user_date.Messages };
        }

        if (!rest_user_date.Response[0].IsAdmin && rest_user_date.Response[0].Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Contains(x)) != true)
        {
            loggerRepo.LogError($"Для получения обращений не достаточно прав: {mem_key}");
            return new()
            {
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Обращение не найдено или у вас нет к нему доступа" }]
            };
        }

        cache.Set(mem_key, issues_db, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));
        return new() { Response = issues_db };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> MessageUpdateOrCreate(TAuthRequestModel<IssueMessageHelpdeskBaseModel> req)
    {
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<int?> res = new();

        if (string.IsNullOrWhiteSpace(req.Payload.MessageText))
        {
            res.AddError("Пустой текст сообщения");
            return res;
        }
        req.Payload.MessageText = req.Payload.MessageText.Trim();

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = req.SenderActionUserId,
            Payload = new() { IssuesIds = [req.Payload.IssueId], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null || issues_data.Response.Length == 0)
            return new() { Messages = issues_data.Messages };

        TResponseModel<UserInfoModel[]?> rest = req.SenderActionUserId == GlobalStaticConstants.Roles.System
            ? new() { Response = [UserInfoModel.BuildSystem()] }
            : await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);

        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        if (!actor.IsAdmin && actor.UserId != GlobalStaticConstants.Roles.System && actor.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Any(y => y == x)) != true && !issues_data.Response.All(iss => actor.UserId == iss.AuthorIdentityUserId))
        {
            res.AddError("У вас не достаточно прав");
            return res;
        }
        IssueHelpdeskModelDB issue_data = issues_data.Response.Single();
        if (req.SenderActionUserId != GlobalStaticConstants.Roles.System && issue_data.Subscribers?.Any(x => x.UserId == req.SenderActionUserId) != true)
        {
            await SubscribeUpdate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    IssueId = issue_data.Id,
                    SetValue = true,
                    UserId = actor.UserId,
                    IsSilent = false,
                }
            });
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IssueMessageHelpdeskModelDB msg_db;
        IssueReadMarkerHelpdeskModelDB? my_marker;
        DateTime dtn = DateTime.UtcNow;
        string msg;
        PulseRequestModel p_req;
        if (req.Payload.Id < 1)
        {
            msg_db = new()
            {
                AuthorUserId = actor.UserId,
                CreatedAt = dtn,
                LastUpdateAt = dtn,
                MessageText = req.Payload.MessageText,
                IssueId = req.Payload.IssueId,
            };
            msg = "Сообщение успешно отправлено";
            await context.AddAsync(msg_db);
            await context.SaveChangesAsync();
            res.AddSuccess(msg);

            res.Response = msg_db.Id;
            if (actor.UserId != GlobalStaticConstants.Roles.System)
            {
                p_req = new()
                {
                    Payload = new()
                    {
                        Payload = new()
                        {
                            IssueId = issue_data.Id,
                            PulseType = PulseIssuesTypesEnum.Messages,
                            Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME,
                            Description = $"Пользователь `{actor.UserName}` добавил комментарий в обращение #{issue_data.Id} '{issue_data.Name}'",
                        },
                        SenderActionUserId = req.SenderActionUserId,
                    },
                    IsMuteEmail = true,
                    IsMuteTelegram = true,
                    IsMuteWhatsapp = true,
                };

                await PulsePush(p_req);

                my_marker = await context.IssueReadMarkers.FirstOrDefaultAsync(x => x.IssueId == req.Payload.IssueId && x.UserIdentityId == actor.UserId);
                if (my_marker is null)
                {
                    my_marker = new()
                    {
                        LastReadAt = dtn,
                        UserIdentityId = actor.UserId,
                        IssueId = req.Payload.IssueId,
                    };
                    await context.AddAsync(my_marker);
                    await context.SaveChangesAsync();
                }
                else
                {
                    await context.IssueReadMarkers.Where(x => x.Id == my_marker.Id)
                        .ExecuteUpdateAsync(set => set
                        .SetProperty(p => p.LastReadAt, dtn));
                }

                await context
                    .IssueReadMarkers
                    .Where(x => x.IssueId == req.Payload.IssueId && x.Id != my_marker.Id)
                    .ExecuteDeleteAsync();

                OrdersByIssuesSelectRequestModel req_docs = new()
                {
                    IssueIds = [issue_data.Id],
                };

                TResponseModel<OrderDocumentModelDB[]> find_orders = await commRepo.OrdersByIssues(req_docs);
                if (find_orders.Success() && find_orders.Response is not null && find_orders.Response.Length != 0)
                {
                    TResponseModel<TelegramBotConfigModel?> wc = await webTransmissionRepo.GetWebConfig();
                    OrderDocumentModelDB order_obj = find_orders.Response[0];
                    string _about_order = $"'{order_obj.Name}' {order_obj.CreatedAtUTC.GetCustomTime().ToString("d", cultureInfo)} {order_obj.CreatedAtUTC.GetCustomTime().ToString("t", cultureInfo)}";

                    string ReplaceTags(string raw, bool clearMd = false)
                    {
                        return raw.Replace(GlobalStaticConstants.OrderDocumentName, order_obj.Name)
                        .Replace(GlobalStaticConstants.OrderDocumentDate, $"{order_obj.CreatedAtUTC.GetCustomTime().ToString("d", cultureInfo)} {order_obj.CreatedAtUTC.GetCustomTime().ToString("t", cultureInfo)}")
                        .Replace(GlobalStaticConstants.OrderStatusInfo, issue_data.StepIssue.DescriptionInfo())
                        .Replace(GlobalStaticConstants.OrderLinkAddress, clearMd ? $"{wc.Response?.ClearBaseUri}/issue-card/{order_obj.HelpdeskId}" : $"<a href='{wc.Response?.ClearBaseUri}/issue-card/{order_obj.HelpdeskId}'>{_about_order}</a>")
                        .Replace(GlobalStaticConstants.HostAddress, clearMd ? wc.Response?.ClearBaseUri : $"<a href='{wc.Response?.ClearBaseUri}'>{wc.Response?.ClearBaseUri}</a>");
                    }

                    string subject_email = "Новое сообщение";
                    TResponseModel<string?> CommerceNewMessageOrderSubjectNotification = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewMessageOrderSubjectNotification);
                    if (CommerceNewMessageOrderSubjectNotification.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderSubjectNotification.Response))
                        subject_email = CommerceNewMessageOrderSubjectNotification.Response;
                    subject_email = ReplaceTags(subject_email);

                    msg = $"<p>Заказ '{order_obj.Name}' от [{order_obj.CreatedAtUTC.GetCustomTime()}]: Новое сообщение.</p>" +
                                        $"<p>/<a href='{wc.Response?.ClearBaseUri}'>{wc.Response?.ClearBaseUri}</a>/</p>";

                    string tg_message = msg.Replace("<p>", "\n").Replace("</p>", "");

                    TResponseModel<string?> CommerceNewMessageOrderBodyNotification = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewMessageOrderBodyNotification);
                    if (CommerceNewMessageOrderBodyNotification.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderBodyNotification.Response))
                        msg = CommerceNewMessageOrderBodyNotification.Response;
                    msg = ReplaceTags(msg);

                    TResponseModel<string?> CommerceNewMessageOrderBodyNotificationTelegram = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewMessageOrderBodyNotificationTelegram);
                    if (CommerceNewMessageOrderBodyNotificationTelegram.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderBodyNotificationTelegram.Response))
                        tg_message = CommerceNewMessageOrderBodyNotificationTelegram.Response;
                    tg_message = ReplaceTags(tg_message);

                    IQueryable<SubscriberIssueHelpdeskModelDB> _qs = issue_data.Subscribers!.Where(x => !x.IsSilent).AsQueryable();

                    string[] users_ids = [.. _qs.Select(x => x.UserId).Union([issue_data.AuthorIdentityUserId, issue_data.ExecutorIdentityUserId]).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()];
                    TResponseModel<UserInfoModel[]?> users_notify = await webTransmissionRepo.GetUsersIdentity(users_ids);
                    if (users_notify.Success() && users_notify.Response is not null && users_notify.Response.Length != 0)
                    {
                        foreach (UserInfoModel u in users_notify.Response)
                        {
                            if (u.TelegramId.HasValue)
                            {
                                TResponseModel<MessageComplexIdsModel?> tgs_res = await telegramRemoteRepo.SendTextMessageTelegram(new()
                                {
                                    Message = tg_message,
                                    UserTelegramId = u.TelegramId!.Value
                                });
                            }
                            loggerRepo.LogInformation(tg_message.Replace("<b>", "").Replace("</b>", ""));
                            await webTransmissionRepo.SendEmail(new() { Email = u.Email!, Subject = subject_email, TextMessage = msg });
                        }
                    }
                }

            }
            else
                await context
                .IssueReadMarkers
                .Where(x => x.IssueId == req.Payload.IssueId)
                .ExecuteDeleteAsync();
        }
        else
        {
            res.Response = 0;
            msg_db = await context.IssuesMessages.FirstAsync(x => x.Id == req.Payload.Id);

            if (msg_db.MessageText == req.Payload.MessageText)
                res.AddInfo("Изменений нет");
            else if (!actor.IsAdmin && msg_db.AuthorUserId != actor.UserId)
            {
                res.AddError("Не достаточно прав");
            }
            else
            {
                res.Response = await context
                    .IssuesMessages
                    .Where(x => x.Id == msg_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.MessageText, req.Payload.MessageText)
                    .SetProperty(p => p.LastUpdateAt, dtn));

                p_req = new()
                {
                    Payload = new()
                    {
                        Payload = new()
                        {
                            IssueId = issue_data.Id,
                            PulseType = PulseIssuesTypesEnum.Messages,
                            Tag = GlobalStaticConstants.Routes.CHANGE_ACTION_NAME,
                            Description = $"Пользователь `{actor.UserName}` изменил комментарий #{msg_db.Id}.",
                        },
                        SenderActionUserId = req.SenderActionUserId,
                    },
                    IsMuteEmail = true,
                    IsMuteTelegram = true,
                    IsMuteWhatsapp = true,
                };

                await PulsePush(p_req);

                msg = "Сообщение успешно обновлено";
                res.AddSuccess(msg);
            }
        }
        await ConsoleSegmentCacheEmpty(issue_data.StepIssue);
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> MessageVote(TAuthRequestModel<VoteIssueRequestModel> req)
    {
        TResponseModel<bool?> res = new();
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<UserInfoModel[]?> rest = req.SenderActionUserId == GlobalStaticConstants.Roles.System
            ? new() { Response = [UserInfoModel.BuildSystem()] }
            : await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);

        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IssueMessageHelpdeskModelDB msg_db = await context.IssuesMessages.FirstAsync(x => x.Id == req.Payload.MessageId);

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssuesIds = [msg_db.IssueId], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null || issues_data.Response.Length != 1)
            return new() { Messages = issues_data.Messages };

        if (!actor.IsAdmin && actor.UserId != GlobalStaticConstants.Roles.System && actor.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Any(y => y == x)) != true && !issues_data.Response.All(iss => actor.UserId == iss.AuthorIdentityUserId))
        {
            res.AddError("У вас не достаточно прав");
            return res;
        }
        var issue_data = issues_data.Response.Single();
        if (req.SenderActionUserId != GlobalStaticConstants.Roles.System && issue_data.Subscribers?.Any(x => x.UserId == req.SenderActionUserId) != true)
        {
            await SubscribeUpdate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    IssueId = issue_data.Id,
                    SetValue = true,
                    UserId = actor.UserId,
                    IsSilent = false,
                }
            });
        }

        int? vote_db_key = await context
            .Votes
            .Where(x => x.MessageId == msg_db.Id && x.IdentityUserId == req.SenderActionUserId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
        PulseRequestModel p_req;
        if (req.Payload.SetStatus)
        {
            if (!vote_db_key.HasValue)
            {
                VoteHelpdeskModelDB vote_db = new() { IdentityUserId = actor.UserId, IssueId = issue_data.Id, MessageId = msg_db.Id };
                await context.AddAsync(vote_db);
                await context.SaveChangesAsync();

                res.AddSuccess("Ваш голос учтён");
                p_req = new()
                {
                    Payload = new()
                    {
                        SenderActionUserId = req.SenderActionUserId,
                        Payload = new()
                        {
                            IssueId = issue_data.Id,
                            PulseType = PulseIssuesTypesEnum.Vote,
                            Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME,
                            Description = $"Пользователь `{actor.UserName}` проголосовал за сообщение #{msg_db.Id}",
                        }
                    }
                };

                await PulsePush(p_req);
            }
            else
                res.AddInfo("Вы уже проголосовали");
        }
        else
        {
            if (!vote_db_key.HasValue)
                res.AddInfo("Ваш голос отсутствует");
            else
            {
                await context
                    .Votes
                    .Where(x => x.Id == vote_db_key.Value)
                    .ExecuteDeleteAsync();

                res.AddInfo("Ваш голос удалён");
                p_req = new()
                {
                    Payload = new()
                    {
                        SenderActionUserId = req.SenderActionUserId,
                        Payload = new()
                        {
                            IssueId = issue_data.Id,
                            PulseType = PulseIssuesTypesEnum.Vote,
                            Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                            Description = $"Пользователь `{actor.UserName}` удалил свой голос за сообщение #{msg_db.Id}",
                        }
                    }
                };

                await PulsePush(p_req);
            }
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> PulsePush(PulseRequestModel req)
    {
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<bool> res = new()
        {
            Response = false,
        };

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = GlobalStaticConstants.Roles.System,
            Payload = new() { IssuesIds = [req.Payload.Payload.IssueId], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null || issues_data.Response.Length == 0)
            return new() { Messages = issues_data.Messages };

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        await context.AddAsync(new PulseIssueModelDB()
        {
            AuthorUserIdentityId = req.Payload.SenderActionUserId,
            Description = req.Payload.Payload.Description,
            CreatedAt = DateTime.UtcNow,
            IssueId = req.Payload.Payload.IssueId,
            PulseType = req.Payload.Payload.PulseType,
            Tag = req.Payload.Payload.Tag,
        });
        await context.SaveChangesAsync();
        res.Response = true;

        PulseIssuesTypesEnum[] _notifiesTypes = [PulseIssuesTypesEnum.Status, PulseIssuesTypesEnum.Subscribes, PulseIssuesTypesEnum.Messages, PulseIssuesTypesEnum.Files];
        if (!_notifiesTypes.Contains(req.Payload.Payload.PulseType))
            return res;
        else if ((req.Payload.Payload.PulseType == PulseIssuesTypesEnum.Messages || req.Payload.Payload.PulseType == PulseIssuesTypesEnum.Subscribes) && req.Payload.Payload.Tag != GlobalStaticConstants.Routes.ADD_ACTION_NAME)
            return res;

        IssueHelpdeskModelDB issue_data = issues_data.Response.Single();
        await cacheRepo.RemoveAsync(GlobalStaticConstants.Cache.ConsoleSegmentStatusToken(issue_data.StepIssue));

        List<string> users_ids = [issue_data.AuthorIdentityUserId];
        if (!string.IsNullOrWhiteSpace(issue_data.ExecutorIdentityUserId))
            users_ids.Add(issue_data.ExecutorIdentityUserId);
        if (issue_data.Subscribers is not null && issue_data.Subscribers.Count != 0)
            users_ids.AddRange(issue_data.Subscribers.Where(x => !x.IsSilent).Select(x => x.UserId));

        users_ids = [.. users_ids.Distinct()];
        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.GetUsersIdentity([.. users_ids]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != users_ids.Count)
            return new() { Messages = rest.Messages };

        List<Task> tasks = [];

        TResponseModel<string?>? wappiToken = null, wappiProfileId = null;

        if (!req.IsMuteEmail || !req.IsMuteTelegram || !req.IsMuteWhatsapp)
            foreach (UserInfoModel user in rest.Response)
            {
                string _subj = $"Уведомление: {req.Payload.Payload.PulseType.DescriptionInfo()}";
                if (!req.IsMuteEmail)
                    tasks.Add(webTransmissionRepo.SendEmail(new() { Email = user.Email!, Subject = _subj, TextMessage = req.Payload.Payload.Description }));

                if (user.TelegramId.HasValue && !req.IsMuteTelegram)
                {
                    SendTextMessageTelegramBotModel tg_req = new()
                    {
                        From = _subj,
                        Message = req.Payload.Payload.Description,
                        UserTelegramId = user.TelegramId.Value,
                        ParseModeName = "html"
                    };
                    tasks.Add(telegramRemoteRepo.SendTextMessageTelegram(tg_req));
                }

                if (!string.IsNullOrWhiteSpace(user.PhoneNumber) && GlobalTools.IsPhoneNumber(user.PhoneNumber) && !req.IsMuteWhatsapp)
                {
                    wappiToken ??= await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.WappiTokenApi);
                    wappiProfileId ??= await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.WappiProfileId);

                    if (wappiToken.Success() && !string.IsNullOrWhiteSpace(wappiToken.Response) && wappiProfileId.Success() && !string.IsNullOrWhiteSpace(wappiProfileId.Response))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Wappi.ToString());
                            if (!client.DefaultRequestHeaders.Any(x => x.Key == "Authorization"))
                                client.DefaultRequestHeaders.Add("Authorization", wappiToken.Response);

                            using HttpResponseMessage response = await client.PostAsJsonAsync($"/api/sync/message/send?profile_id={wappiProfileId.Response}", new SendMessageRequestModel() { Body = req.Payload.Payload.Description, Recipient = user.PhoneNumber });
                            string rj = await response.Content.ReadAsStringAsync();
                            SendMessageResponseModel sendWappiRes = JsonConvert.DeserializeObject<SendMessageResponseModel>(rj)!;
                        }));
                    }
                }
            }

        if (tasks.Count != 0)
            await Task.WhenAll(tasks);

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> StatusChange(TAuthRequestModel<StatusChangeRequestModel> req)
    {
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<bool> res = new()
        {
            Response = false,
        };

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (req.SenderActionUserId != GlobalStaticConstants.Roles.System && (!rest.Success() || rest.Response is null || rest.Response.Length != 1))
            return new() { Messages = rest.Messages };

        UserInfoModel actor = req.SenderActionUserId == GlobalStaticConstants.Roles.System ? UserInfoModel.BuildSystem() : rest.Response![0];

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssuesIds = [req.Payload.IssueId], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null || issues_data.Response.Length == 0)
            return new() { Messages = issues_data.Messages };

        IssueHelpdeskModelDB issue_data = issues_data.Response.Single();

        if (!actor.IsAdmin &&
            issue_data.AuthorIdentityUserId != actor.UserId &&
            issue_data.ExecutorIdentityUserId != actor.UserId &&
            actor.UserId != GlobalStaticConstants.Roles.System &&
            actor.UserId != GlobalStaticConstants.Roles.HelpDeskTelegramBotManager)
        {
            res.AddError("Не достаточно прав для смены статуса");
            return res;
        }

        if (req.SenderActionUserId != GlobalStaticConstants.Roles.System && issue_data.Subscribers?.Any(x => x.UserId == req.SenderActionUserId) != true)
        {
            await SubscribeUpdate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    IssueId = issue_data.Id,
                    SetValue = true,
                    UserId = actor.UserId,
                    IsSilent = false,
                }
            });
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        if (issue_data.StepIssue == req.Payload.Step)
            res.AddInfo("Статус уже установлен");
        else
        {
            if (string.IsNullOrWhiteSpace(issue_data.ExecutorIdentityUserId) &&
                req.Payload.Step >= StatusesDocumentsEnum.Progress &&
                req.Payload.Step != StatusesDocumentsEnum.Canceled)
            {
                res.AddError("Для перевода обращения в работу нужно сначала указать исполнителя");
                return res;
            }

            string msg = $"Статус успешно изменён с `{issue_data.StepIssue}` на `{req.Payload.Step}`";

            await context.Issues.Where(x => x.Id == issue_data.Id)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.StepIssue, req.Payload.Step)
                .SetProperty(p => p.LastUpdateAt, DateTime.UtcNow));

            await ConsoleSegmentCacheEmpty(issue_data.StepIssue);
            res.AddSuccess(msg);
            res.Response = true;
            PulseRequestModel p_req = new()
            {
                Payload = new()
                {
                    SenderActionUserId = req.SenderActionUserId,
                    Payload = new()
                    {
                        IssueId = issue_data.Id,
                        PulseType = PulseIssuesTypesEnum.Status,
                        Tag = req.Payload.Step.DescriptionInfo(),
                        Description = msg,
                    },
                },
                IsMuteEmail = true,
                IsMuteTelegram = true,
                IsMuteWhatsapp = true,
            };

            await PulsePush(p_req);
            OrdersByIssuesSelectRequestModel req_docs = new()
            {
                IssueIds = [issue_data.Id],
            };

            TResponseModel<OrderDocumentModelDB[]> find_orders = await commRepo.OrdersByIssues(req_docs);
            if (find_orders.Success() && find_orders.Response is not null && find_orders.Response.Length != 0)
            {
                await commRepo.StatusOrderChange(new() { IssueId = issue_data.Id, Step = req.Payload.Step });
                TResponseModel<TelegramBotConfigModel?> wc = await webTransmissionRepo.GetWebConfig();
                OrderDocumentModelDB order_obj = find_orders.Response[0];
                DateTime cdd = order_obj.CreatedAtUTC.GetCustomTime();
                string normCreatedAtUTC = $"{cdd.ToString("d", cultureInfo)} {cdd.ToString("t", cultureInfo)}";
                string _about_order = $"'{order_obj.Name}' {normCreatedAtUTC}";
                string ReplaceTags(string raw, bool clearMd = false)
                {
                    return raw.Replace(GlobalStaticConstants.OrderDocumentName, order_obj.Name)
                    .Replace(GlobalStaticConstants.OrderDocumentDate, $"{cdd.ToString("d", cultureInfo)} {cdd.ToString("t", cultureInfo)}")
                    .Replace(GlobalStaticConstants.OrderStatusInfo, req.Payload.Step.DescriptionInfo())
                    .Replace(GlobalStaticConstants.OrderLinkAddress, clearMd ? $"{wc.Response?.ClearBaseUri}/issue-card/{order_obj.HelpdeskId}" : $"<a href='{wc.Response?.ClearBaseUri}/issue-card/{order_obj.HelpdeskId}'>{_about_order}</a>")
                    .Replace(GlobalStaticConstants.HostAddress, clearMd ? wc.Response?.ClearBaseUri : $"<a href='{wc.Response?.ClearBaseUri}'>{wc.Response?.ClearBaseUri}</a>");
                }

                string subject_email = "Изменение статуса документа";
                TResponseModel<string?> CommerceStatusChangeOrderSubjectNotification = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceStatusChangeOrderSubjectNotification(issue_data.StepIssue));
                if (CommerceStatusChangeOrderSubjectNotification.Success() && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderSubjectNotification.Response))
                    subject_email = CommerceStatusChangeOrderSubjectNotification.Response;
                subject_email = ReplaceTags(subject_email);

                msg = $"<p>Заказ '{order_obj.Name}' от [{normCreatedAtUTC}] - {req.Payload.Step.DescriptionInfo()}.</p>" +
                                    $"<p>/<a href='{wc.Response?.ClearBaseUri}'>{wc.Response?.ClearBaseUri}</a>/</p>";

                string tg_message = msg.Replace("<p>", "\n").Replace("</p>", "");
                string wp_message = $"Заказ '{order_obj.Name}' от [{normCreatedAtUTC}] - {req.Payload.Step.DescriptionInfo()}. " +
                                    $"{wc.Response?.ClearBaseUri}";

                TResponseModel<string?> CommerceStatusChangeOrderBodyNotification = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceStatusChangeOrderBodyNotification(issue_data.StepIssue));
                if (CommerceStatusChangeOrderBodyNotification.Success() && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderBodyNotification.Response))
                    msg = CommerceStatusChangeOrderBodyNotification.Response;
                msg = ReplaceTags(msg);

                TResponseModel<string?> CommerceStatusChangeOrderBodyNotificationTelegram = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceStatusChangeOrderBodyNotificationTelegram(issue_data.StepIssue));
                if (CommerceStatusChangeOrderBodyNotificationTelegram.Success() && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderBodyNotificationTelegram.Response))
                    tg_message = CommerceStatusChangeOrderBodyNotificationTelegram.Response;
                tg_message = ReplaceTags(tg_message);

                TResponseModel<string?>? CommerceStatusChangeOrderBodyNotificationWhatsapp = null;

                IQueryable<SubscriberIssueHelpdeskModelDB> _qs = issue_data.Subscribers!.Where(x => !x.IsSilent).AsQueryable();
                TResponseModel<string?>? wappiToken = null, wappiProfileId = null;
                List<Task> tasks = [];
                string[] users_ids = [.. _qs.Select(x => x.UserId).Union([issue_data.AuthorIdentityUserId, issue_data.ExecutorIdentityUserId]).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()];
                TResponseModel<UserInfoModel[]?> users_notify = await webTransmissionRepo.GetUsersIdentity(users_ids);
                if (users_notify.Success() && users_notify.Response is not null && users_notify.Response.Length != 0)
                {
                    foreach (UserInfoModel u in users_notify.Response)
                    {
                        if (u.TelegramId.HasValue)
                        {
                            tasks.Add(telegramRemoteRepo.SendTextMessageTelegram(new()
                            {
                                Message = tg_message,
                                UserTelegramId = u.TelegramId!.Value
                            }));
                        }
                        loggerRepo.LogInformation(tg_message.Replace("<b>", "").Replace("</b>", ""));
                        tasks.Add(webTransmissionRepo.SendEmail(new() { Email = u.Email!, Subject = subject_email, TextMessage = msg }));
                        if (!string.IsNullOrWhiteSpace(u.PhoneNumber) && GlobalTools.IsPhoneNumber(u.PhoneNumber))
                        {
                            if (CommerceStatusChangeOrderBodyNotificationWhatsapp is null)
                            {
                                CommerceStatusChangeOrderBodyNotificationWhatsapp = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceStatusChangeOrderBodyNotificationWhatsapp(issue_data.StepIssue));
                                if (CommerceStatusChangeOrderBodyNotificationWhatsapp.Success() && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderBodyNotificationWhatsapp.Response))
                                    wp_message = CommerceStatusChangeOrderBodyNotificationWhatsapp.Response;
                                wp_message = ReplaceTags(wp_message, true);
                            }

                            wappiToken ??= await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.WappiTokenApi);
                            wappiProfileId ??= await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.WappiProfileId);

                            if (wappiToken.Success() && !string.IsNullOrWhiteSpace(wappiToken.Response) && wappiProfileId.Success() && !string.IsNullOrWhiteSpace(wappiProfileId.Response))
                            {
                                tasks.Add(Task.Run(async () =>
                                {
                                    using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Wappi.ToString());
                                    if (!client.DefaultRequestHeaders.Any(x => x.Key == "Authorization"))
                                        client.DefaultRequestHeaders.Add("Authorization", wappiToken.Response);

                                    using HttpResponseMessage response = await client.PostAsJsonAsync($"/api/sync/message/send?profile_id={wappiProfileId.Response}", new SendMessageRequestModel() { Body = wp_message, Recipient = u.PhoneNumber });
                                    string rj = await response.Content.ReadAsStringAsync();
                                    SendMessageResponseModel sendWappiRes = JsonConvert.DeserializeObject<SendMessageResponseModel>(rj)!;
                                }));
                            }
                        }
                    }
                    if (tasks.Count != 0)
                        await Task.WhenAll(tasks);
                }
            }
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> SubscribeUpdate(TAuthRequestModel<SubscribeUpdateRequestModel> req)
    {
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<bool?> res = new() { Response = false };

        string[] users_ids = [req.SenderActionUserId, req.Payload.UserId];
        users_ids = [.. users_ids.Distinct()];

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.GetUsersIdentity(users_ids);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != users_ids.Length)
            return new() { Messages = rest.Messages };

        UserInfoModel
            actor = rest.Response.First(x => x.UserId == req.SenderActionUserId),
            requested_user = rest.Response.First(x => x.UserId == req.Payload.UserId);

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new IssuesReadRequestModel()
            {
                IssuesIds = [req.Payload.IssueId],
                IncludeSubscribersOnly = true,
            },
        });

        if (!issues_data.Success() || issues_data.Response is null || issues_data.Response.Length == 0)
            return new() { Messages = issues_data.Messages };
        IssueHelpdeskModelDB issue_data = issues_data.Response.Single();
        if (actor.UserId != GlobalStaticConstants.Roles.System && !actor.IsAdmin && actor.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Any(y => y == x)) != true && actor.UserId != issue_data.AuthorIdentityUserId)
        {
            res.AddError("У вас не достаточно прав для выполнения этой операции");
            return res;
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        var sdb = await context
             .SubscribersOfIssues
             .Where(x => x.IssueId == issue_data.Id && x.UserId == requested_user.UserId)
             .Select(x => new { x.Id, x.IsSilent })
             .FirstOrDefaultAsync();

        PulseRequestModel p_req;
        string msg;
        if (req.Payload.SetValue)
        {
            if (sdb is null)
            {
                msg = "Подписка успешно добавлена";
                await context.SubscribersOfIssues.AddAsync(new() { UserId = requested_user.UserId, IssueId = issue_data.Id, IsSilent = req.Payload.IsSilent });
                await context.SaveChangesAsync();
                res.AddSuccess(msg);

                if (req.SenderActionUserId != GlobalStaticConstants.Roles.System)
                {
                    p_req = new()
                    {
                        Payload = new()
                        {
                            SenderActionUserId = req.SenderActionUserId,
                            Payload = new()
                            {
                                IssueId = issue_data.Id,
                                PulseType = PulseIssuesTypesEnum.Subscribes,
                                Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME,
                                Description = $"Пользователь `{requested_user.UserName}` добавлен в подписчики",
                            }
                        }
                    };

                    await PulsePush(p_req);
                }
            }
            else
            {
                if (req.Payload.IsSilent == sdb.IsSilent)
                    res.AddInfo("Подписка уже существует");
                else
                {
                    await context
                        .SubscribersOfIssues
                        .Where(x => x.Id == sdb.Id)
                        .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.IsSilent, req.Payload.IsSilent));
                    msg = $"Уведомления успешно {(req.Payload.IsSilent ? "отключены" : "включены")} для: {requested_user.UserName}";
                    res.AddSuccess(msg);

                    if (req.SenderActionUserId != GlobalStaticConstants.Roles.System)
                    {
                        p_req = new()
                        {
                            Payload = new()
                            {
                                SenderActionUserId = req.SenderActionUserId,
                                Payload = new()
                                {
                                    IssueId = issue_data.Id,
                                    PulseType = PulseIssuesTypesEnum.Subscribes,
                                    Tag = GlobalStaticConstants.Routes.CHANGE_ACTION_NAME,
                                    Description = msg,
                                }
                            }
                        };

                        await PulsePush(p_req);
                    }
                }
            }
        }
        else
        {
            if (sdb is null)
                res.AddInfo("Подписки нет");
            else
            {
                await context.SubscribersOfIssues
                    .Where(x => x.Id == sdb.Id)
                    .ExecuteDeleteAsync();
                msg = "Подписка успешно удалена";
                res.AddSuccess(msg);

                if (req.SenderActionUserId != GlobalStaticConstants.Roles.System)
                {
                    p_req = new()
                    {
                        Payload = new()
                        {
                            SenderActionUserId = req.SenderActionUserId,
                            Payload = new()
                            {
                                IssueId = issue_data.Id,
                                PulseType = PulseIssuesTypesEnum.Subscribes,
                                Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                                Description = $"Пользователь `{requested_user.UserName}` удалён из подписок",
                            }
                        }
                    };

                    await PulsePush(p_req);
                }
            }
        }
        res.Response = true;
        await ConsoleSegmentCacheEmpty(issue_data.StepIssue);
        return res;
    }

    /// <inheritdoc/>
    public async Task ConsoleSegmentCacheEmpty(StatusesDocumentsEnum st)
    {
        MemCacheComplexKeyModel mceKey = GlobalStaticConstants.Cache.ConsoleSegmentStatusToken(st);
        string? cacheToken = await cacheRepo.GetStringValueAsync(mceKey);
        if (!string.IsNullOrWhiteSpace(cacheToken))
        {
            await cacheRepo.RemoveAsync(cacheToken);
            await cacheRepo.RemoveAsync(mceKey);
        }
    }
}