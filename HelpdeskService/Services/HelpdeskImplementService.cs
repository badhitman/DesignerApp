////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using HtmlAgilityPack;
using Newtonsoft.Json;
using SharedLib;
using DbcLib;
using Microsoft.EntityFrameworkCore.Storage;

namespace HelpdeskService;

/// <summary>
/// Helpdesk - Implement
/// </summary>
public class HelpdeskImplementService(
    IIdentityTransmission IdentityRepo,
    ILogger<HelpdeskImplementService> loggerRepo,
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IManualCustomCacheService cacheRepo,
    IOptions<HelpdeskConfigModel> hdConf,
    ICommerceTransmission commRepo,
    IMemoryCache cache,
    ITelegramTransmission telegramRemoteRepo,
    IStorageTransmission StorageRepo,
    IWebTransmission webTransmissionRepo) : IHelpdeskService
{
    static readonly TimeSpan _ts = TimeSpan.FromSeconds(5);

    #region messages
    /// <inheritdoc/>
    public async Task<TResponseModel<IssueMessageHelpdeskModelDB[]>> MessagesList(TAuthRequestModel<int> req)
    {
        TResponseModel<IssueMessageHelpdeskModelDB[]> res = new();

        TResponseModel<UserInfoModel[]> rest = await IdentityRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssuesIds = [req.Payload], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null)
            return new() { Messages = issues_data.Messages };

        if (!actor.IsAdmin && actor.UserId != GlobalStaticConstants.Roles.System && actor.Roles?.Any(role_actor => GlobalStaticConstants.Roles.AllHelpDeskRoles.Any(hd_role => hd_role == role_actor)) != true && !issues_data.Response.All(c => actor.UserId == c.AuthorIdentityUserId))
        {
            res.AddError("У вас не достаточно прав");
            return res;
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        int[] issues_ids = issues_data.Response.Select(i => i.Id).ToArray();

        res.Response = await context
            .IssuesMessages
            .Where(x => issues_ids.Any(y => y == x.IssueId))
            .OrderByDescending(i => i.CreatedAt)
            .Include(x => x.Votes)
            .ToArrayAsync();

        return res;
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

        TResponseModel<UserInfoModel[]> rest = req.SenderActionUserId == GlobalStaticConstants.Roles.System
            ? new() { Response = [UserInfoModel.BuildSystem()] }
            : await IdentityRepo.GetUsersIdentity([req.SenderActionUserId]);

        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        if (!actor.IsAdmin && actor.UserId != GlobalStaticConstants.Roles.System && actor.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Any(y => y == x)) != true && !issues_data.Response.All(iss => actor.UserId == iss.AuthorIdentityUserId))
        {
            res.AddError("У вас не достаточно прав");
            return res;
        }
        List<Task> tasks = [];
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
        IssueReadMarkerHelpdeskModelDB? my_marker = null;
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
            msg = "Сообщение успешно добавлено к документу";
            await context.AddAsync(msg_db);
            await context.SaveChangesAsync();
            res.AddInfo(msg);

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
                            Description = $"Пользователь `{actor.UserName}` в обращение #{issue_data.Id} '{issue_data.Name}' добавил комментарий: {req.Payload.MessageText}",
                        },
                        SenderActionUserId = req.SenderActionUserId,
                    },
                    IsMuteEmail = true,
                    IsMuteTelegram = true,
                    IsMuteWhatsapp = true,
                };

                tasks = [
                    PulsePush(p_req),
                    Task.Run(async () => { my_marker = await context.IssueReadMarkers.FirstOrDefaultAsync(x => x.IssueId == req.Payload.IssueId && x.UserIdentityId == actor.UserId); })];

                await Task.WhenAll(tasks);
                tasks.Clear();

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

                tasks.Add(context
                    .IssueReadMarkers
                    .Where(x => x.IssueId == req.Payload.IssueId && x.Id != my_marker.Id)
                    .ExecuteDeleteAsync());

                OrdersByIssuesSelectRequestModel req_docs = new()
                {
                    IssueIds = [issue_data.Id],
                };

                TelegramBotConfigModel wc = default!;
                TResponseModel<OrderDocumentModelDB[]> find_orders = default!;
                TResponseModel<string?> CommerceNewMessageOrderBodyNotificationWhatsapp = default!, CommerceNewMessageOrderSubjectNotification = default!, CommerceNewMessageOrderBodyNotification = default!, CommerceNewMessageOrderBodyNotificationTelegram = default!;

                string safeTextMessage = req.Payload.MessageText.Replace("<p>", "").Replace("</p>", "\n").Trim();
                safeTextMessage = safeTextMessage.Contains('>') || safeTextMessage.Contains('<')
                    ? ""
                    : safeTextMessage;

                if (string.IsNullOrWhiteSpace(safeTextMessage))
                {
                    HtmlDocument doc = new();
                    doc.LoadHtml(req.Payload.MessageText);
                    safeTextMessage = doc.DocumentNode.InnerText;
                }
                if (safeTextMessage.Length > 128)
                    safeTextMessage = $"{safeTextMessage[..125]}...";

                string subject_email = "Новое сообщение";
                string _about_document = $"Обращение '{issue_data.Name}' от [{issue_data.CreatedAtUTC.GetHumanDateTime()}]";
                string wpMessage = $"Заявка '{issue_data.Name}' от [{issue_data.CreatedAtUTC.GetHumanDateTime()}]: Пользователь `{actor.UserName}` добавил комментарий.";
                msg = $"<p>{_about_document}: Пользователь `{actor.UserName}` добавил комментарий.</p>";
                string tg_message = msg.Replace("<p>", "\n").Replace("</p>", "");

                tasks.Add(Task.Run(async () => { find_orders = await commRepo.OrdersByIssues(req_docs); }));
                tasks.Add(Task.Run(async () => { wc = await webTransmissionRepo.GetWebConfig(); }));
                tasks.Add(Task.Run(async () =>
                {
                    CommerceNewMessageOrderBodyNotificationWhatsapp = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewMessageOrderBodyNotificationWhatsapp);
                    if (CommerceNewMessageOrderBodyNotificationWhatsapp.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderBodyNotificationWhatsapp.Response))
                        wpMessage = IHelpdeskService.ReplaceTags(issue_data.Name, issue_data.CreatedAtUTC, issue_data.Id, issue_data.StatusDocument, CommerceNewMessageOrderBodyNotificationWhatsapp.Response, wc.ClearBaseUri, _about_document, true);
                }));
                tasks.Add(Task.Run(async () =>
                {
                    CommerceNewMessageOrderBodyNotificationTelegram = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewMessageOrderBodyNotificationTelegram);
                    if (CommerceNewMessageOrderBodyNotificationTelegram.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderBodyNotificationTelegram.Response))
                        tg_message = IHelpdeskService.ReplaceTags(issue_data.Name, issue_data.CreatedAtUTC, issue_data.Id, issue_data.StatusDocument, CommerceNewMessageOrderBodyNotificationTelegram.Response, wc.ClearBaseUri, _about_document);
                }));
                tasks.Add(Task.Run(async () =>
                {
                    CommerceNewMessageOrderBodyNotification = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewMessageOrderBodyNotification);
                    if (CommerceNewMessageOrderBodyNotification.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderBodyNotification.Response))
                        msg = IHelpdeskService.ReplaceTags(issue_data.Name, issue_data.CreatedAtUTC, issue_data.Id, issue_data.StatusDocument, CommerceNewMessageOrderBodyNotification.Response, wc.ClearBaseUri, _about_document);
                }));
                tasks.Add(Task.Run(async () =>
                {
                    CommerceNewMessageOrderSubjectNotification = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewMessageOrderSubjectNotification);
                    if (CommerceNewMessageOrderSubjectNotification.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderSubjectNotification.Response))
                        subject_email = IHelpdeskService.ReplaceTags(issue_data.Name, issue_data.CreatedAtUTC, issue_data.Id, issue_data.StatusDocument, CommerceNewMessageOrderSubjectNotification.Response, wc.ClearBaseUri, _about_document);
                }));
                await Task.WhenAll(tasks);
                tasks.Clear();

                IQueryable<SubscriberIssueHelpdeskModelDB> _qs = issue_data.Subscribers!.Where(x => !x.IsSilent).AsQueryable();

                string[] users_ids = [.. _qs.Select(x => x.UserId).Union([issue_data.AuthorIdentityUserId, issue_data.ExecutorIdentityUserId]).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()!]; ;

                if (find_orders.Success() && find_orders.Response is not null && find_orders.Response.Length != 0)
                {
                    OrderDocumentModelDB order_obj = find_orders.Response[0];
                    _about_document = $"Заказ '{order_obj.Name}' {order_obj.CreatedAtUTC.GetCustomTime().ToString("d", GlobalStaticConstants.RU)} {order_obj.CreatedAtUTC.GetCustomTime().ToString("t", GlobalStaticConstants.RU)}";
                    if (CommerceNewMessageOrderSubjectNotification.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderSubjectNotification.Response))
                        subject_email = IHelpdeskService.ReplaceTags(order_obj.Name, order_obj.CreatedAtUTC, order_obj.HelpdeskId!.Value, order_obj.StatusDocument, CommerceNewMessageOrderSubjectNotification.Response, wc.ClearBaseUri, _about_document);

                    if (!CommerceNewMessageOrderBodyNotification.Success() || string.IsNullOrWhiteSpace(CommerceNewMessageOrderBodyNotification.Response))
                        msg = $"<p>Заказ '{order_obj.Name}' от [{order_obj.CreatedAtUTC.GetHumanDateTime()}]: Новое сообщение.</p>";
                    else
                        msg = IHelpdeskService.ReplaceTags(order_obj.Name, order_obj.CreatedAtUTC, order_obj.HelpdeskId!.Value, order_obj.StatusDocument, CommerceNewMessageOrderBodyNotification.Response, wc.ClearBaseUri, _about_document);

                    if (CommerceNewMessageOrderBodyNotificationTelegram.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderBodyNotificationTelegram.Response))
                        tg_message = IHelpdeskService.ReplaceTags(order_obj.Name, order_obj.CreatedAtUTC, order_obj.HelpdeskId!.Value, order_obj.StatusDocument, CommerceNewMessageOrderBodyNotificationTelegram.Response, wc.ClearBaseUri, _about_document);

                    if (CommerceNewMessageOrderBodyNotificationWhatsapp.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderBodyNotificationWhatsapp.Response))
                        wpMessage = IHelpdeskService.ReplaceTags(order_obj.Name, order_obj.CreatedAtUTC, order_obj.HelpdeskId!.Value, order_obj.StatusDocument, CommerceNewMessageOrderBodyNotificationWhatsapp.Response, wc.ClearBaseUri, _about_document, true);
                    else
                        wpMessage = $"Заказ '{order_obj.Name}' от [{order_obj.CreatedAtUTC.GetHumanDateTime()}]: Новое сообщение.";

                    users_ids = [.. users_ids.Union([order_obj.AuthorIdentityUserId]).Distinct()];
                }
                wpMessage = $"{wpMessage}\n\n> {safeTextMessage}".Trim().TrimEnd('>').Trim();

                TResponseModel<UserInfoModel[]> users_notify = await IdentityRepo.GetUsersIdentity(users_ids);
                if (users_notify.Success() && users_notify.Response is not null && users_notify.Response.Length != 0)
                {
                    foreach (UserInfoModel u in users_notify.Response)
                    {
                        loggerRepo.LogInformation(tg_message.Replace("<b>", "").Replace("</b>", ""));
                        tasks.Add(IdentityRepo.SendEmail(new() { Email = u.Email!, Subject = subject_email, TextMessage = $"{msg}</hr>{req.Payload.MessageText}" }, false));

                        if (u.TelegramId.HasValue)
                        {
                            SendTextMessageTelegramBotModel tg_req = new()
                            {
                                From = subject_email,
                                Message = $"{tg_message}\n\n<code>{safeTextMessage}</code>".Trim(),
                                UserTelegramId = u.TelegramId.Value,
                                ParseModeName = "html"
                            };
                            tasks.Add(telegramRemoteRepo.SendTextMessageTelegram(tg_req, false));
                        }

                        if (!string.IsNullOrWhiteSpace(u.PhoneNumber) && GlobalTools.IsPhoneNumber(u.PhoneNumber))
                            tasks.Add(telegramRemoteRepo.SendWappiMessage(new() { Number = u.PhoneNumber, Text = wpMessage }, false));
                    }
                }

                if (tasks.Count != 0)
                    await Task.WhenAll(tasks);
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
                res.AddError("Не достаточно прав");
            else
            {
                p_req = new()
                {
                    Payload = new()
                    {
                        Payload = new()
                        {
                            IssueId = issue_data.Id,
                            PulseType = PulseIssuesTypesEnum.Messages,
                            Tag = GlobalStaticConstants.Routes.CHANGE_ACTION_NAME,
                            Description = $"Пользователь <a href='/Users/Profiles/view-{actor.UserId}' target='_blank'>{actor.UserName}</a> изменил комментарий #{msg_db.Id}.<br /><dl><dt>старое:</dt><dd>{msg_db.MessageText}</dd><dt>новое:</dt><dd>{req.Payload.MessageText}</dd></dl>",
                        },
                        SenderActionUserId = req.SenderActionUserId,
                    },
                    IsMuteEmail = true,
                    IsMuteTelegram = true,
                    IsMuteWhatsapp = true,
                };

                tasks.Add(Task.Run(async () =>
                {
                    res.Response = await context
                        .IssuesMessages
                        .Where(x => x.Id == msg_db.Id)
                        .ExecuteUpdateAsync(set => set
                        .SetProperty(p => p.MessageText, req.Payload.MessageText)
                        .SetProperty(p => p.LastUpdateAt, dtn));
                }));
                tasks.Add(PulsePush(p_req));
                await Task.WhenAll(tasks);
                msg = "Сообщение успешно обновлено";
                res.AddSuccess(msg);
            }
        }
        await ConsoleSegmentCacheEmpty(issue_data.StatusDocument);
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> MessageVote(TAuthRequestModel<VoteIssueRequestModel> req)
    {
        TResponseModel<bool?> res = new();
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<UserInfoModel[]> rest = req.SenderActionUserId == GlobalStaticConstants.Roles.System
            ? new() { Response = [UserInfoModel.BuildSystem()] }
            : await IdentityRepo.GetUsersIdentity([req.SenderActionUserId]);

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

    #endregion

    #region rubric
    /// <inheritdoc/>
    public async Task<List<UniversalBaseModel>> RubricsList(RubricsListRequestModel req)
    {
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IQueryable<UniversalBaseModel> q = context
            .Rubrics
            .Where(x => x.ProjectId == req.ProjectId && x.ContextName == req.ContextName)
            .Select(x => new UniversalBaseModel()
            {
                Name = x.Name,
                Description = x.Description,
                Id = x.Id,
                IsDisabled = x.IsDisabled,
                ParentId = x.ParentId,
                ProjectId = x.ProjectId,
                SortIndex = x.SortIndex,
            })
            .AsQueryable();

        if (req.Request < 1)
            q = q.Where(x => x.ParentId == null || x.ParentId < 1);
        else
            q = q.Where(x => x.ParentId == req.Request);

        return await q.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> RubricMove(TAuthRequestModel<RowMoveModel> req)
    {
        ResponseBaseModel res = new();

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        var data = await context
        .Rubrics
        .Where(x => x.Id == req.Payload.ObjectId)
        .Select(x => new { x.Id, x.ParentId, x.Name })
        .FirstAsync(x => x.Id == req.Payload.ObjectId);

        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        LockUniqueTokenModelDB locker = new() { Token = $"rubric-sort-upd-{data.ParentId}" };
        try
        {
            await context.AddAsync(locker);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            res.AddError($"Не удалось выполнить команду: {ex.Message}");
            return res;
        }

        List<RubricIssueHelpdeskModelDB> all = await context
            .Rubrics
            .Where(x => x.ContextName == req.Payload.ContextName && x.ParentId == data.ParentId)
            .OrderBy(x => x.SortIndex)
            .ToListAsync();

        int i = all.FindIndex(x => x.Id == data.Id);
        if (req.Payload.Direction == DirectionsEnum.Up)
        {
            if (i == 0)
            {
                res.AddInfo("Элемент уже в крайнем положении.");
            }
            else
            {
                RubricIssueHelpdeskModelDB r1 = all[i - 1], r2 = all[i];
                uint val1 = r1.SortIndex, val2 = r2.SortIndex;
                r1.SortIndex = uint.MaxValue;
                context.Update(r1);
                await context.SaveChangesAsync();
                r2.SortIndex = val1;
                context.Update(r2);
                await context.SaveChangesAsync();
                r1.SortIndex = val2;
                context.Update(r1);
                await context.SaveChangesAsync();

                res.AddSuccess($"Рубрика '{data.Name}' сдвинута выше");
            }
        }
        else
        {
            if (i == all.Count - 1)
            {
                res.AddInfo("Элемент уже в крайнем положении.");
            }
            else
            {
                RubricIssueHelpdeskModelDB r1 = all[i + 1], r2 = all[i];
                uint val1 = r1.SortIndex, val2 = r2.SortIndex;
                r1.SortIndex = uint.MaxValue;
                context.Update(r1);
                await context.SaveChangesAsync();
                r2.SortIndex = val1;
                context.Update(r2);
                await context.SaveChangesAsync();
                r1.SortIndex = val2;
                context.Update(r1);
                await context.SaveChangesAsync();

                res.AddSuccess($"Рубрика '{data.Name}' сдвинута ниже");
            }
        }

        all = [.. all.OrderBy(x => x.SortIndex)];

        bool nu = false;
        uint si = 0;
        all.ForEach(x =>
        {
            si++;
            nu = nu || x.SortIndex != si;
            x.SortIndex = si;
        });

        if (nu)
        {
            context.UpdateRange(all);
        }
        context.Remove(locker);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RubricCreateOrUpdate(RubricIssueHelpdeskModelDB rubric)
    {
        TResponseModel<int> res = new();
        Regex rx = new(@"\s+", RegexOptions.Compiled);
        rubric.Name = rx.Replace(rubric.Name.Trim(), " ");
        if (string.IsNullOrWhiteSpace(rubric.Name))
        {
            res.AddError("Объект должен иметь имя");
            return res;
        }
        rubric.NormalizedNameUpper = rubric.Name.ToUpper();
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        if (await context.Rubrics.AnyAsync(x => x.Id != rubric.Id && x.ParentId == rubric.ParentId && x.Name == rubric.Name))
        {
            res.AddError("Объект с таким именем уже существует в данном узле");
            return res;
        }

        if (rubric.Id < 1)
        {
            uint[] six = await context
                            .Rubrics
                            .Where(x => x.ParentId == rubric.ParentId)
                            .Select(x => x.SortIndex)
                            .ToArrayAsync();

            rubric.SortIndex = six.Length == 0 ? 1 : six.Max() + 1;

            await context.AddAsync(rubric);
            await context.SaveChangesAsync();
            res.AddSuccess("Объект успешно создан");
            res.Response = rubric.Id;
        }
        else
        {
            res.Response = await context
                .Rubrics
                .Where(x => x.Id == rubric.Id)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.IsDisabled, rubric.IsDisabled)
                .SetProperty(p => p.Name, rubric.Name)
                .SetProperty(p => p.Description, rubric.Description));

            res.AddSuccess("Объект успешно обновлён");
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<List<RubricIssueHelpdeskModelDB>>> RubricRead(int rubricId)
    {
        TResponseModel<List<RubricIssueHelpdeskModelDB>> res = new();

        if (rubricId < 1)
            return res;

        string mem_key = $"{GlobalStaticConstants.TransmissionQueues.RubricForIssuesReadHelpdeskReceive}-{rubricId}";
        if (cache.TryGetValue(mem_key, out List<RubricIssueHelpdeskModelDB>? rubric))
        {
            res.Response = rubric;
            return res;
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        RubricIssueHelpdeskModelDB? lpi = await context
            .Rubrics
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.Id == rubricId);

        if (lpi is null)
        {
            res.AddWarning($"Рубрика #{rubricId} не найдена в БД (вероятно была удалена)");
            return res;
        }

        List<RubricIssueHelpdeskModelDB> ctrl = [lpi];

        while (lpi.Parent is not null)
        {
            ctrl.Add(await context
            .Rubrics
            .Include(x => x.Parent)
            .ThenInclude(x => x!.NestedRubrics)
            .FirstAsync(x => x.Id == lpi.Parent.Id));
            lpi = ctrl.Last();
        }

        res.Response = ctrl;
        cache.Set(mem_key, res.Response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<List<RubricIssueHelpdeskModelDB>>> RubricsGet(int[] rubricsIds)
    {
        TResponseModel<List<RubricIssueHelpdeskModelDB>> res = new();
        rubricsIds = [.. rubricsIds.Where(x => x > 0)];
        if (rubricsIds.Length == 0)
        {
            res.AddError("Пустой запрос");
            return res;
        }
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        res.Response = await context.Rubrics.Where(x => rubricsIds.Any(y => y == x.Id)).ToListAsync();
        return res;
    }
    #endregion

    #region issues
    /// <inheritdoc/>
    public async Task<TResponseModel<List<SubscriberIssueHelpdeskModelDB>>> SubscribesList(TAuthRequestModel<int> req)
    {
        TResponseModel<UserInfoModel[]> rest = await IdentityRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssuesIds = [req.Payload], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null)
            return new() { Messages = issues_data.Messages };

        return new() { Response = [.. issues_data.Response.SelectMany(x => x.Subscribers!)] };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>>> IssuesSelect(TAuthRequestModel<TPaginationRequestModel<SelectIssuesRequestModel>> req)
    {
        if (req.Payload.PageSize < 5)
            req.Payload.PageSize = 5;

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IQueryable<IssueHelpdeskModelDB> q = context
            .Issues
            .Where(x => x.ProjectId == req.Payload.Payload.ProjectId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload.Payload.SearchQuery))
        {
            req.Payload.Payload.SearchQuery = req.Payload.Payload.SearchQuery.ToUpper();

            q = from issue_element in q
                join rubric_element in context.Rubrics on issue_element.RubricIssueId equals rubric_element.Id
                into grp_rubrics
                from c in grp_rubrics.DefaultIfEmpty()
                where issue_element.NormalizedNameUpper!.Contains(req.Payload.Payload.SearchQuery) || c.NormalizedNameUpper!.Contains(req.Payload.Payload.SearchQuery)
                select issue_element;
        }

        switch (req.Payload.Payload.JournalMode)
        {
            case HelpdeskJournalModesEnum.ActualOnly:
                q = q.Where(x => x.StatusDocument <= StatusesDocumentsEnum.Progress);
                break;
            case HelpdeskJournalModesEnum.ArchiveOnly:
                q = q.Where(x => x.StatusDocument > StatusesDocumentsEnum.Progress);
                break;
            default:
                break;
        }

        switch (req.Payload.Payload.UserArea)
        {
            case UsersAreasHelpdeskEnum.Subscriber:
                q = q.Where(x => context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && req.Payload.Payload.IdentityUsersIds.Contains(y.UserId)));
                break;
            case UsersAreasHelpdeskEnum.Executor:
                q = q.Where(x => req.Payload.Payload.IdentityUsersIds.Contains(x.ExecutorIdentityUserId));
                break;
            case UsersAreasHelpdeskEnum.Main:
                q = q.Where(x => req.Payload.Payload.IdentityUsersIds.Contains(x.ExecutorIdentityUserId) || context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && req.Payload.Payload.IdentityUsersIds.Contains(y.UserId)));
                break;
            case UsersAreasHelpdeskEnum.Author:
                q = q.Where(x => req.Payload.Payload.IdentityUsersIds.Contains(x.AuthorIdentityUserId));
                break;
            default:
                if (req.Payload.Payload.UserArea is not null)
                    q = q.Where(x => req.Payload.Payload.IdentityUsersIds.Contains(x.AuthorIdentityUserId) || req.Payload.Payload.IdentityUsersIds.Contains(x.ExecutorIdentityUserId) || context.SubscribersOfIssues.Any(y => y.IssueId == x.Id && req.Payload.Payload.IdentityUsersIds.Contains(y.UserId)));
                break;
        }

        q = req.Payload.SortingDirection == DirectionsEnum.Up
            ? q.OrderBy(x => x.CreatedAtUTC)
            : q.OrderByDescending(x => x.CreatedAtUTC);

        var inc = q
            .Skip(req.Payload.PageNum * req.Payload.PageSize)
            .Take(req.Payload.PageSize)
            .Include(x => x.RubricIssue);

        List<IssueHelpdeskModelDB> data = req.Payload.Payload.IncludeSubscribers
            ? await inc.Include(x => x.Subscribers).ToListAsync()
            : await inc.ToListAsync();

        return new()
        {
            Response = new()
            {
                PageNum = req.Payload.PageNum,
                PageSize = req.Payload.PageSize,
                SortingDirection = req.Payload.SortingDirection,
                SortBy = req.Payload.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = [.. data.Select(x => IssueHelpdeskModel.Build(x))]
            }
        };
    }

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
            .Where(x => x.ProjectId == req.Payload.ProjectId && x.StatusDocument == req.Payload.Status)
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

        IOrderedQueryable<IssueHelpdeskModelDB> oq = req.SortingDirection == DirectionsEnum.Up
            ? q.OrderBy(x => x.CreatedAtUTC)
            : q.OrderByDescending(x => x.CreatedAtUTC);

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
            await cacheRepo.SetObjectAsync(cacheToken, res, TimeSpan.FromSeconds(hdConf.Value.ConsoleSegmentCacheLifetimeSeconds));

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

        TResponseModel<UserInfoModel[]> users_rest = await IdentityRepo.GetUsersIdentity(users_ids);
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
                if (issue_data.StatusDocument >= StatusesDocumentsEnum.Progress)
                {
                    res.AddError($"Обращение в статусе [{issue_data.StatusDocument.DescriptionInfo()}]. После того как обращение переходит в работу (и далее) удалить исполнителя нельзя. Для открепления исполнителя поставьте обращение на паузу");
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
        await ConsoleSegmentCacheEmpty(issue_data.StatusDocument);
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> IssueCreateOrUpdate(TAuthRequestModel<UniversalUpdateRequestModel> issue_upd)
    {
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(issue_upd)}");
        TResponseModel<int> res = new() { Response = 0 };
        TResponseModel<ModesSelectRubricsEnum?> res_ModeSelectingRubrics = default!;
        TResponseModel<UserInfoModel[]> users_rest = default!;

        List<Task> tasks = [
            Task.Run(async () => { users_rest = await IdentityRepo.GetUsersIdentity([issue_upd.SenderActionUserId]); }),
            Task.Run(async () => { res_ModeSelectingRubrics = await StorageRepo.ReadParameter<ModesSelectRubricsEnum?>(GlobalStaticConstants.CloudStorageMetadata.ModeSelectingRubrics); }) ];

        await Task.WhenAll(tasks);
        tasks.Clear();

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

        ModesSelectRubricsEnum _current_mode_rubric = res_ModeSelectingRubrics.Response ?? ModesSelectRubricsEnum.AllowWithoutRubric;
        if (_current_mode_rubric != ModesSelectRubricsEnum.AllowWithoutRubric)
        {
            string[] sub_rubrics = await context
                        .Rubrics
                        .Where(x => x.ParentId == issue_upd.Payload.ParentId)
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
                RubricIssueId = issue_upd.Payload.ParentId,
                NormalizedNameUpper = normalizedNameUpper,
                NormalizedDescriptionUpper = normalizedDescriptionUpper,
                StatusDocument = StatusesDocumentsEnum.Created,
                ProjectId = issue_upd.Payload.ProjectId,
                CreatedAtUTC = dtn,
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
                        Tag = issue.StatusDocument.DescriptionInfo(),
                        Description = msg,
                    },
                    SenderActionUserId = GlobalStaticConstants.Roles.System,
                },
                IsMuteEmail = true,
                IsMuteTelegram = true,
                IsMuteWhatsapp = true,
            };
            TResponseModel<long?> helpdesk_user_redirect_telegram_for_issue_rest = default!;
            await Task.WhenAll([
                PulsePush(p_req),
                Task.Run(async () => { helpdesk_user_redirect_telegram_for_issue_rest = await StorageRepo.ReadParameter<long?>(GlobalStaticConstants.CloudStorageMetadata.HelpdeskNotificationTelegramForCreateIssue); }),
                MessageUpdateOrCreate(new() { SenderActionUserId = GlobalStaticConstants.Roles.System, Payload = new() { MessageText = $"Пользователь `{actor.UserName}` создал новый запрос: {issue_upd.Payload.Name}", IssueId = issue.Id } })]);

            if (helpdesk_user_redirect_telegram_for_issue_rest.Success() && helpdesk_user_redirect_telegram_for_issue_rest.Response.HasValue && helpdesk_user_redirect_telegram_for_issue_rest.Response != 0)
            {
                await telegramRemoteRepo.SendTextMessageTelegram(new()
                {
                    Message = $"Создана новая заявка: #{issue.Id} '{issue.Name}'. Автор: {actor}",
                    From = "уведомление",
                    UserTelegramId = helpdesk_user_redirect_telegram_for_issue_rest.Response.Value,
                }, false);
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
                                .SetProperty(b => b.RubricIssueId, issue_upd.Payload.ParentId)
                                .SetProperty(b => b.Name, issue_upd.Payload.Name)
                                .SetProperty(b => b.LastUpdateAt, DateTime.UtcNow));

                OrdersByIssuesSelectRequestModel req_comm = new()
                {
                    IncludeExternalData = true,
                    IssueIds = [issue_upd.Payload.Id],
                };

                TelegramBotConfigModel wc = default!;
                TResponseModel<OrderDocumentModelDB[]> comm_res = default!;
                TResponseModel<RecordsAttendanceModelDB[]> attendance_res = default!;

                await Task.WhenAll([
                    Task.Run(async () => { comm_res = await commRepo.OrdersByIssues(req_comm); }),
                    Task.Run(async () => { attendance_res = await commRepo.OrdersAttendancesByIssues(req_comm); }),
                    Task.Run(async () => { wc = await webTransmissionRepo.GetWebConfig(); })]);

                msg = $"Документ (#{issue_upd.Payload.Id}) обновлён.";
                if (comm_res.Success() && comm_res.Response is not null && comm_res.Response.Length != 0)
                {
                    msg += $". Заказ: [{string.Join(";", comm_res.Response.Select(x => $"(№{x.Id} - {x.CreatedAtUTC.GetCustomTime()})"))}].\n";
                }
                if (attendance_res.Success() && attendance_res.Response is not null && attendance_res.Response.Length != 0)
                {
                    msg += $". Услуги: [{string.Join(";", attendance_res.Response.Select(x => $"(№{x.Id} - {x.CreatedAtUTC.GetCustomTime()})"))}].\n";
                }

                msg += $". /<a href='{wc.ClearBaseUri}'>{wc.ClearBaseUri}</a>/";
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
                tasks = [PulsePush(p_req)];
                if (issue_upd.SenderActionUserId != GlobalStaticConstants.Roles.System && issue.Subscribers?.Any(x => x.UserId == issue_upd.SenderActionUserId) != true)
                {
                    tasks.Add(SubscribeUpdate(new()
                    {
                        SenderActionUserId = GlobalStaticConstants.Roles.System,
                        Payload = new()
                        {
                            IssueId = issue.Id,
                            SetValue = true,
                            UserId = actor.UserId,
                            IsSilent = false,
                        }
                    }));
                }
                await Task.WhenAll(tasks);
            }
            else
                res.AddError($"У вас не достаточно прав для редактирования этого обращения #{issue_upd.Payload.Id} '{issue.Name}'");
        }
        await ConsoleSegmentCacheEmpty(issue.StatusDocument);
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
            await ConsoleSegmentCacheEmpty();
            return new()
            {
                Messages = [new() { TypeMessage = ResultTypesEnum.Warning, Text = "Обращение не найдено или у вас нет к нему доступа" }]
            };
        }

        if (req.SenderActionUserId == GlobalStaticConstants.Roles.System || issues_db.All(x => x.ExecutorIdentityUserId == req.SenderActionUserId) || issues_db.All(x => x.AuthorIdentityUserId == req.SenderActionUserId) || issues_db.All(x => x.Subscribers!.Any(x => x.UserId == req.SenderActionUserId)))
            return new() { Response = issues_db };

        TResponseModel<UserInfoModel[]> rest_user_date = await IdentityRepo.GetUsersIdentity([req.SenderActionUserId]);
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
    public async Task<TResponseModel<bool>> IssueStatusChange(TAuthRequestModel<StatusChangeRequestModel> req)
    {
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<bool> res = new()
        {
            Response = false,
        };

        TResponseModel<UserInfoModel[]> rest = await IdentityRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (req.SenderActionUserId != GlobalStaticConstants.Roles.System && (!rest.Success() || rest.Response is null || rest.Response.Length != 1))
            return new() { Messages = rest.Messages };

        UserInfoModel actor = req.SenderActionUserId == GlobalStaticConstants.Roles.System ? UserInfoModel.BuildSystem() : rest.Response![0];

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssuesIds = [req.Payload.DocumentId], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null || issues_data.Response.Length == 0)
            return new() { Messages = issues_data.Messages };

        IssueHelpdeskModelDB issue_data = issues_data.Response.Single();
        StatusesDocumentsEnum prevStatus = issue_data.StatusDocument;
        StatusesDocumentsEnum nextStatus = req.Payload.Step;

        if (prevStatus == nextStatus)
        {
            res.AddInfo("Статус уже установлен");
            await commRepo.StatusOrderChangeByHelpdeskDocumentId(new() { Payload = new() { DocumentId = issue_data.Id, Step = nextStatus, }, SenderActionUserId = req.SenderActionUserId }, false);
            return res;
        }

        if (string.IsNullOrWhiteSpace(issue_data.ExecutorIdentityUserId) &&
            nextStatus >= StatusesDocumentsEnum.Progress &&
            nextStatus != StatusesDocumentsEnum.Canceled)
        {
            res.AddError("Для перевода обращения в работу нужно сначала указать исполнителя");
            return res;
        }

        List<string> users_ids = [issue_data.AuthorIdentityUserId];

        users_ids = issue_data
            .Subscribers!
            .Where(x => !x.IsSilent)
            .Select(x => x.UserId)
            .Union([issue_data.AuthorIdentityUserId])
            .Distinct()
            .ToList();

        if (!string.IsNullOrWhiteSpace(issue_data.ExecutorIdentityUserId))
            users_ids.Add(issue_data.ExecutorIdentityUserId);

        if (!actor.IsAdmin &&
            issue_data.AuthorIdentityUserId != actor.UserId &&
            issue_data.ExecutorIdentityUserId != actor.UserId &&
            actor.UserId != GlobalStaticConstants.Roles.System &&
            actor.UserId != GlobalStaticConstants.Roles.HelpDeskTelegramBotManager)
        {
            res.AddError("Не достаточно прав для смены статуса");
            return res;
        }

        TResponseModel<string?>
            CommerceStatusChangeOrderSubjectNotification = default!,
            CommerceStatusChangeOrderBodyNotification = default!,
            CommerceStatusChangeOrderBodyNotificationWhatsapp = default!,
            CommerceStatusChangeOrderBodyNotificationTelegram = default!;

        TelegramBotConfigModel wc = default!;
        List<Task> tasks = [
            Task.Run(async () => { CommerceStatusChangeOrderSubjectNotification = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceStatusChangeOrderSubjectNotification(prevStatus)); }),
            Task.Run(async () => { CommerceStatusChangeOrderBodyNotification = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceStatusChangeOrderBodyNotification(prevStatus)); }),
            Task.Run(async () => { CommerceStatusChangeOrderBodyNotificationTelegram = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceStatusChangeOrderBodyNotificationTelegram(prevStatus)); }),
            Task.Run(async () => { CommerceStatusChangeOrderBodyNotificationWhatsapp = await StorageRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceStatusChangeOrderBodyNotificationWhatsapp(prevStatus)); }),
            Task.Run(async () => { wc = await webTransmissionRepo.GetWebConfig(); })];

        if (req.SenderActionUserId != GlobalStaticConstants.Roles.System && issue_data.Subscribers?.Any(x => x.UserId == req.SenderActionUserId) != true)
        {
            tasks.Add(SubscribeUpdate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    IssueId = issue_data.Id,
                    SetValue = true,
                    UserId = actor.UserId,
                    IsSilent = false,
                }
            }));
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        tasks.Add(context.Issues.Where(x => x.Id == issue_data.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.StatusDocument, nextStatus)
            .SetProperty(p => p.LastUpdateAt, DateTime.UtcNow)));

        await Task.WhenAll(tasks);
        tasks.Clear();

        DateTime cdd = issue_data.CreatedAtUTC.GetCustomTime();
        string _about_document = $"'{issue_data.Name}' {cdd.GetHumanDateTime()}";
        string subject_email = "Изменение статуса документа";
        if (CommerceStatusChangeOrderSubjectNotification?.Success() == true && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderSubjectNotification.Response))
            subject_email = IHelpdeskService.ReplaceTags(issue_data.Name, cdd, issue_data.Id, nextStatus, CommerceStatusChangeOrderSubjectNotification.Response, wc.ClearBaseUri, _about_document);

        string msg = $"<p>Заявка '{_about_document} [изменение статуса]: `{prevStatus.DescriptionInfo()}` → `{nextStatus.DescriptionInfo()}`" +
                            $"<p>/<a href='{wc.ClearBaseUri}'>{wc.ClearBaseUri}</a>/</p>";

        if (CommerceStatusChangeOrderBodyNotification?.Success() == true && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderBodyNotification.Response))
            msg = IHelpdeskService.ReplaceTags(issue_data.Name, cdd, issue_data.Id, nextStatus, CommerceStatusChangeOrderBodyNotification.Response, wc.ClearBaseUri, _about_document);

        string tg_message = msg.Replace("<p>", "\n").Replace("</p>", "");
        if (CommerceStatusChangeOrderBodyNotificationTelegram?.Success() == true && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderBodyNotificationTelegram.Response))
            tg_message = IHelpdeskService.ReplaceTags(issue_data.Name, cdd, issue_data.Id, nextStatus, CommerceStatusChangeOrderBodyNotificationTelegram.Response, wc.ClearBaseUri, _about_document);

        string wp_message = $"Заявка '{_about_document} [изменение статуса]: `{prevStatus.DescriptionInfo()}` → `{nextStatus.DescriptionInfo()}`" +
                            $"{wc.ClearBaseUri}";

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
                    Tag = nextStatus.DescriptionInfo(),
                    Description = msg,
                },
            },
            IsMuteEmail = true,
            IsMuteTelegram = true,
            IsMuteWhatsapp = true,
        };
        tasks.Add(PulsePush(p_req));

        OrdersByIssuesSelectRequestModel req_docs = new()
        {
            IssueIds = [issue_data.Id],
        };
        TResponseModel<OrderDocumentModelDB[]> find_orders = default!;
        TResponseModel<RecordsAttendanceModelDB[]> find_orders_attendances = default!;

        await Task.WhenAll([
                Task.Run(async () => find_orders = await commRepo.OrdersByIssues(req_docs)),
                Task.Run(async () => find_orders_attendances = await commRepo.OrdersAttendancesByIssues(req_docs))
            ]);

        bool order_exist = find_orders.Success() && find_orders.Response is not null && find_orders.Response.Length != 0;
        bool order_attendance_exist = find_orders_attendances.Success() && find_orders_attendances.Response is not null && find_orders_attendances.Response.Length != 0;

        if (order_exist || order_attendance_exist)
        {
            if (order_exist)
                await commRepo.StatusOrderChangeByHelpdeskDocumentId(new() { Payload = new() { DocumentId = issue_data.Id, Step = nextStatus, }, SenderActionUserId = req.SenderActionUserId }, false);

            if (order_attendance_exist)
                await commRepo.StatusesOrdersAttendancesChangeByHelpdeskDocumentId(new() { SenderActionUserId = req.SenderActionUserId, Payload = new() { DocumentId = issue_data.Id, Step = nextStatus, } }, false);

            OrderDocumentModelDB? order_obj = find_orders.Response?.FirstOrDefault();
            RecordsAttendanceModelDB? order_attendance = find_orders_attendances.Response?.FirstOrDefault();

            if (order_obj is not null && !users_ids.Contains(order_obj.AuthorIdentityUserId))
                users_ids.Add(order_obj.AuthorIdentityUserId);

            if (order_attendance is not null && !users_ids.Contains(order_attendance.AuthorIdentityUserId))
                users_ids.Add(order_attendance.AuthorIdentityUserId);

            string _docName = order_obj?.Name ?? order_attendance?.Name ?? "-no name-";
            int _hdDocId = order_obj?.HelpdeskId ?? order_attendance?.HelpdeskId ?? 0;

            _about_document = $"'{_docName}' {cdd.GetHumanDateTime()}";

            if (CommerceStatusChangeOrderSubjectNotification?.Success() == true && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderSubjectNotification.Response))
                subject_email = IHelpdeskService.ReplaceTags(_docName, cdd, _hdDocId, nextStatus, CommerceStatusChangeOrderSubjectNotification.Response, wc.ClearBaseUri, _about_document);

            msg = $"<p>Заказ '{_about_document} [изменение статуса]: `{prevStatus}` → `{nextStatus}`" +
                                $"<p>/<a href='{wc.ClearBaseUri}'>{wc.ClearBaseUri}</a>/</p>";

            tg_message = msg.Replace("<p>", "\n").Replace("</p>", "");
            wp_message = $"Заказ '{_docName}' от [{cdd.GetHumanDateTime()}] - {nextStatus.DescriptionInfo()}. " +
                               $"{wc.ClearBaseUri}";

            if (CommerceStatusChangeOrderBodyNotification?.Success() == true && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderBodyNotification.Response))
                msg = IHelpdeskService.ReplaceTags(_docName, cdd, _hdDocId, nextStatus, CommerceStatusChangeOrderBodyNotification.Response, wc.ClearBaseUri, _about_document);

            if (CommerceStatusChangeOrderBodyNotificationTelegram?.Success() == true && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderBodyNotificationTelegram.Response))
                tg_message = IHelpdeskService.ReplaceTags(_docName, cdd, _hdDocId, nextStatus, CommerceStatusChangeOrderBodyNotificationTelegram.Response, wc.ClearBaseUri, _about_document);
        }

        TResponseModel<UserInfoModel[]> users_notify = await IdentityRepo.GetUsersIdentity(users_ids);
        if (users_notify?.Success() == true && users_notify.Response is not null && users_notify.Response.Length != 0)
        {
            foreach (UserInfoModel u in users_notify.Response)
            {
                tasks.Add(IdentityRepo.SendEmail(new() { Email = u.Email!, Subject = subject_email, TextMessage = msg }, false));
                if (u.TelegramId.HasValue)
                {
                    tasks.Add(telegramRemoteRepo.SendTextMessageTelegram(new()
                    {
                        Message = tg_message,
                        UserTelegramId = u.TelegramId!.Value
                    }, false));
                }
                loggerRepo.LogInformation(tg_message.Replace("<b>", "").Replace("</b>", ""));
                if (!string.IsNullOrWhiteSpace(u.PhoneNumber) && GlobalTools.IsPhoneNumber(u.PhoneNumber))
                {
                    if (CommerceStatusChangeOrderBodyNotificationWhatsapp.Success() && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderBodyNotificationWhatsapp.Response))
                        wp_message = CommerceStatusChangeOrderBodyNotificationWhatsapp.Response;

                    tasks.Add(telegramRemoteRepo.SendWappiMessage(new() { Number = u.PhoneNumber, Text = wp_message }, false));
                }
            }
        }

        tasks.Add(ConsoleSegmentCacheEmpty(prevStatus));
        tasks.Add(ConsoleSegmentCacheEmpty(nextStatus));
        await Task.WhenAll(tasks);

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> SubscribeUpdate(TAuthRequestModel<SubscribeUpdateRequestModel> req)
    {
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<bool?> res = new() { Response = false };

        string[] users_ids = [req.SenderActionUserId, req.Payload.UserId];
        users_ids = [.. users_ids.Distinct()];

        TResponseModel<UserInfoModel[]> rest = await IdentityRepo.GetUsersIdentity(users_ids);
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
        await ConsoleSegmentCacheEmpty(issue_data.StatusDocument);
        return res;
    }
    #endregion

    #region pulse
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
        await cacheRepo.RemoveAsync(GlobalStaticConstants.Cache.ConsoleSegmentStatusToken(issue_data.StatusDocument));

        List<string> users_ids = [issue_data.AuthorIdentityUserId];
        if (!string.IsNullOrWhiteSpace(issue_data.ExecutorIdentityUserId))
            users_ids.Add(issue_data.ExecutorIdentityUserId);
        if (issue_data.Subscribers is not null && issue_data.Subscribers.Count != 0)
            users_ids.AddRange(issue_data.Subscribers.Where(x => !x.IsSilent).Select(x => x.UserId));

        users_ids = [.. users_ids.Distinct()];
        TResponseModel<UserInfoModel[]> rest = await IdentityRepo.GetUsersIdentity([.. users_ids]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != users_ids.Count)
            return new() { Messages = rest.Messages };

        HtmlDocument doc = new();
        doc.LoadHtml(req.Payload.Payload.Description);

        List<Task> tasks = [];
        if (!req.IsMuteEmail || !req.IsMuteTelegram || !req.IsMuteWhatsapp)
            foreach (UserInfoModel user in rest.Response)
            {
                string _subj = $"Уведомление: {req.Payload.Payload.PulseType.DescriptionInfo()}";
                if (!req.IsMuteEmail)
                    tasks.Add(IdentityRepo.SendEmail(new() { Email = user.Email!, Subject = _subj, TextMessage = req.Payload.Payload.Description }, false));

                if (user.TelegramId.HasValue && !req.IsMuteTelegram)
                {
                    SendTextMessageTelegramBotModel tg_req = new()
                    {
                        From = _subj,
                        Message = req.Payload.Payload.Description,
                        UserTelegramId = user.TelegramId.Value,
                        ParseModeName = "html"
                    };
                    tasks.Add(telegramRemoteRepo.SendTextMessageTelegram(tg_req, false));
                }

                if (!string.IsNullOrWhiteSpace(user.PhoneNumber) && GlobalTools.IsPhoneNumber(user.PhoneNumber) && !req.IsMuteWhatsapp)
                    tasks.Add(telegramRemoteRepo.SendWappiMessage(new() { Number = user.PhoneNumber, Text = doc.DocumentNode.InnerText }, false));
            }

        if (tasks.Count != 0)
            await Task.WhenAll(tasks);

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<PulseViewModel>>> PulseJournalSelect(TAuthRequestModel<TPaginationRequestModel<UserIssueModel>> req)
    {
        TResponseModel<UserInfoModel[]> rest = await IdentityRepo.GetUsersIdentity([req.Payload.Payload.UserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length == 0)
            return new() { Messages = rest.Messages };

        if (req.Payload.PageSize < 5)
            req.Payload.PageSize = 5;

        UserInfoModel actor = rest.Response[0];

        loggerRepo.LogDebug($"Запрос журнала активности пользователем: {JsonConvert.SerializeObject(actor)}");

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssuesIds = [req.Payload.Payload.IssueId], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null || issues_data.Response.Length == 0)
        {
            loggerRepo.LogWarning($"Запрос журнала активности пользователем {actor.UserId} - отклонён");
            return new() { Messages = issues_data.Messages };
        }
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IQueryable<PulseIssueModelDB> q = context
            .PulseEvents
            .Where(x => x.IssueId == req.Payload.Payload.IssueId);

        IOrderedQueryable<PulseIssueModelDB> oq = req.Payload.SortingDirection == DirectionsEnum.Down
            ? q.OrderByDescending(x => x.CreatedAt)
            : q.OrderBy(x => x.CreatedAt);

        return new()
        {
            Response = new()
            {
                TotalRowsCount = q.Count(),
                PageNum = req.Payload.PageNum,
                PageSize = req.Payload.PageSize,
                SortBy = req.Payload.SortBy,
                SortingDirection = req.Payload.SortingDirection,
                Response = await oq
                    .Skip(req.Payload.PageSize * req.Payload.PageNum)
                    .Take(req.Payload.PageSize)
                    .Select(x => new PulseViewModel()
                    {
                        AuthorUserIdentityId = x.AuthorUserIdentityId,
                        Description = x.Description,
                        CreatedAt = x.CreatedAt,
                        PulseType = x.PulseType,
                        Tag = x.Tag,
                    })
                    .ToListAsync()
            }
        };
    }
    #endregion

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramMessageIncoming(TelegramIncomingMessageModel req)
    {
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        if (req.ReplyToMessage is not null)
        {
            ForwardMessageTelegramBotModelDB? inc_msg = await context.ForwardedMessages
                .FirstOrDefaultAsync(x =>
                    x.DestinationChatId == req.ReplyToMessage.Chat!.ChatTelegramId &&
                    x.SourceChatId == req.ReplyToMessage.ForwardFromId &&
                    x.ResultMessageTelegramId == req.ReplyToMessage.MessageTelegramId);

            if (inc_msg is not null)
            {
                SendTextMessageTelegramBotModel sender = new()
                {
                    Message = req.Text ?? req.Caption ?? "Вложения",
                    UserTelegramId = inc_msg.SourceChatId,
                    Files = await Files(req),
                    ReplyToMessageId = inc_msg.SourceMessageId,
                };

                TResponseModel<MessageComplexIdsModel> send_answer = await telegramRemoteRepo.SendTextMessageTelegram(sender);

                if (send_answer.Success() && send_answer.Response is not null)
                {
                    await context.AddAsync(new AnswerToForwardModelDB()
                    {
                        ResultMessageId = send_answer.Response.DatabaseId,
                        ResultMessageTelegramId = send_answer.Response.TelegramId,
                        ForwardMessageId = inc_msg.Id,
                    });
                    await context.SaveChangesAsync();
                }
                else
                {
                    sender.ReplyToMessageId = null;
                    send_answer = await telegramRemoteRepo.SendTextMessageTelegram(sender);
                    if (send_answer.Success() && send_answer.Response is not null)
                    {
                        await context.AddAsync(new AnswerToForwardModelDB()
                        {
                            ResultMessageId = send_answer.Response.DatabaseId,
                            ResultMessageTelegramId = send_answer.Response.TelegramId,
                            ForwardMessageId = inc_msg.Id,
                        });
                        await context.SaveChangesAsync();
                    }
                }

                return ResponseBaseModel.CreateInfo("Сообщение является экспресс-ответом клиенту!");
            }
        }

        if (req.Chat?.Type != ChatsTypesTelegramEnum.Private)
            return ResponseBaseModel.CreateWarning("Чат не является частным (обработке в HelpDesk не подлежит)!");

        TResponseModel<long?> helpdesk_user_redirect_telegram_for_issue_rest = await StorageRepo.ReadParameter<long?>(GlobalStaticConstants.CloudStorageMetadata.HelpdeskNotificationsTelegramForUser(req.From!.UserTelegramId));
        if (helpdesk_user_redirect_telegram_for_issue_rest.Success() && helpdesk_user_redirect_telegram_for_issue_rest.Response.HasValue && helpdesk_user_redirect_telegram_for_issue_rest.Response != 0)
        {
            TResponseModel<MessageComplexIdsModel> forward_res = await telegramRemoteRepo.ForwardMessage(new()
            {
                DestinationChatId = helpdesk_user_redirect_telegram_for_issue_rest.Response.Value,
                SourceChatId = req.Chat!.ChatTelegramId,
                SourceMessageId = req.MessageTelegramId,
            });

            if (forward_res.Success() && forward_res.Response is not null)
            {
                await context.AddAsync(new ForwardMessageTelegramBotModelDB()
                {
                    DestinationChatId = helpdesk_user_redirect_telegram_for_issue_rest.Response.Value,
                    ResultMessageId = forward_res.Response.DatabaseId,
                    ResultMessageTelegramId = forward_res.Response.TelegramId,
                    SourceChatId = req.Chat!.ChatTelegramId,
                    SourceMessageId = req.MessageTelegramId,
                });
                await context.SaveChangesAsync();

                return ResponseBaseModel.CreateSuccess($"Сообщение было переслано в чат #{helpdesk_user_redirect_telegram_for_issue_rest.Response.Value} по подписке на пользователя");
            }
            else
                ResponseBaseModel.Create(forward_res.Messages);
        }

        helpdesk_user_redirect_telegram_for_issue_rest = await StorageRepo.ReadParameter<long?>(GlobalStaticConstants.CloudStorageMetadata.HelpdeskNotificationTelegramGlobalForIncomingMessage);
        if (helpdesk_user_redirect_telegram_for_issue_rest.Success() && helpdesk_user_redirect_telegram_for_issue_rest.Response.HasValue && helpdesk_user_redirect_telegram_for_issue_rest.Response != 0)
        {
            TResponseModel<MessageComplexIdsModel> forward_res = await telegramRemoteRepo.ForwardMessage(new()
            {
                DestinationChatId = helpdesk_user_redirect_telegram_for_issue_rest.Response.Value,
                SourceChatId = req.Chat!.ChatTelegramId,
                SourceMessageId = req.MessageTelegramId,
            });

            if (forward_res.Success() && forward_res.Response is not null)
            {
                await context.AddAsync(new ForwardMessageTelegramBotModelDB()
                {
                    DestinationChatId = helpdesk_user_redirect_telegram_for_issue_rest.Response.Value,
                    ResultMessageId = forward_res.Response.DatabaseId,
                    ResultMessageTelegramId = forward_res.Response.TelegramId,
                    SourceChatId = req.Chat!.ChatTelegramId,
                    SourceMessageId = req.MessageTelegramId,
                });
                await context.SaveChangesAsync();
                return ResponseBaseModel.CreateSuccess($"Сообщение было переслано в чат #{helpdesk_user_redirect_telegram_for_issue_rest.Response.Value} по глобальной подписке");
            }
            else
                return ResponseBaseModel.Create(forward_res.Messages);
        }

        return ResponseBaseModel.CreateInfo("Сообщение обработано");
    }

    async Task<List<FileAttachModel>?> Files(TelegramIncomingMessageModel req)
    {
        List<FileAttachModel> files = [];
        TResponseModel<byte[]> data_res;
        //
        if (req.Audio is not null)
        {
            data_res = await telegramRemoteRepo.GetFile(req.Audio.FileId);
            if (data_res.Success() && data_res.Response is not null && data_res.Response.Length != 0)
                files.Add(new() { ContentType = req.Audio.MimeType ?? "application/octet-stream", Data = data_res.Response, Name = req.Audio.FileName ?? req.Audio.Title ?? "Audio" });
        }
        if (req.Document is not null)
        {
            data_res = await telegramRemoteRepo.GetFile(req.Document.FileId);
            if (data_res.Success() && data_res.Response is not null && data_res.Response.Length != 0)
                files.Add(new() { ContentType = req.Document.MimeType ?? "application/octet-stream", Data = data_res.Response, Name = req.Document.FileName ?? "Document" });
        }
        if (req.Photo is not null && req.Photo.Count != 0)
        {
            PhotoMessageTelegramModelDB _f = req.Photo.OrderByDescending(x => x.FileSize).First();
            data_res = await telegramRemoteRepo.GetFile(_f.FileId);
            if (data_res.Success() && data_res.Response is not null && data_res.Response.Length != 0)
                files.Add(new() { ContentType = "image/jpeg", Data = data_res.Response, Name = "Photo" });
        }
        if (req.Voice is not null)
        {
            data_res = await telegramRemoteRepo.GetFile(req.Voice.FileId);
            if (data_res.Success() && data_res.Response is not null && data_res.Response.Length != 0)
                files.Add(new() { ContentType = req.Voice.MimeType ?? "application/octet-stream", Data = data_res.Response, Name = "Voice" });
        }
        if (req.Video is not null)
        {
            data_res = await telegramRemoteRepo.GetFile(req.Video.FileId);
            if (data_res.Success() && data_res.Response is not null && data_res.Response.Length != 0)
                files.Add(new() { ContentType = req.Video.MimeType ?? "application/octet-stream", Data = data_res.Response, Name = req.Video.FileName ?? "Video" });
        }

        return files.Count == 0 ? null : files;
    }

    /// <inheritdoc/>
    public Task<ResponseBaseModel> SetWebConfig(HelpdeskConfigModel req)
    {
        if (!Uri.TryCreate(req.BaseUri, UriKind.Absolute, out _))
            return Task.FromResult(ResponseBaseModel.CreateError("BaseUri is null"));

        return Task.FromResult(hdConf.Value.Update(req.BaseUri));
    }

    /// <inheritdoc/>
    public async Task ConsoleSegmentCacheEmpty(StatusesDocumentsEnum? st = null)
    {
        MemCacheComplexKeyModel mceKey;
        if (st is null || !st.HasValue)
        {
            await Task.WhenAll(Enum.GetValues<StatusesDocumentsEnum>().Cast<StatusesDocumentsEnum>().Select(x =>
            {
                return Task.Run(async () =>
            {
                mceKey = GlobalStaticConstants.Cache.ConsoleSegmentStatusToken(x);
                string? cacheToken = await cacheRepo.GetStringValueAsync(mceKey);
                if (!string.IsNullOrWhiteSpace(cacheToken))
                {
                    await cacheRepo.RemoveAsync(cacheToken);
                    await cacheRepo.RemoveAsync(mceKey);
                }
            });
            }));
            return;
        }

        mceKey = GlobalStaticConstants.Cache.ConsoleSegmentStatusToken(st.Value);
        string? cacheToken = await cacheRepo.GetStringValueAsync(mceKey);
        if (!string.IsNullOrWhiteSpace(cacheToken))
        {
            await cacheRepo.RemoveAsync(cacheToken);
            await cacheRepo.RemoveAsync(mceKey);
        }
    }
}