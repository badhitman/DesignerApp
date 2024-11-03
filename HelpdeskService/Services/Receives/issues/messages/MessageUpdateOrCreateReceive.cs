////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;
using Newtonsoft.Json;
using System.Globalization;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Сообщение в обращение
/// </summary>
public class MessageUpdateOrCreateReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    ILogger<MessageUpdateOrCreateReceive> loggerRepo,
    ICommerceRemoteTransmissionService commRepo,
    ITelegramRemoteTransmissionService tgRepo,
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo,
    IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<IssueMessageHelpdeskBaseModel>?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessageOfIssueUpdateHelpdeskReceive;
    static CultureInfo cultureInfo = new("ru-RU");

    /// <summary>
    /// Сообщение в обращение
    /// </summary>
    public async Task<TResponseModel<int?>> ResponseHandleAction(TAuthRequestModel<IssueMessageHelpdeskBaseModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<int?> res = new();

        if (string.IsNullOrWhiteSpace(req.Payload.MessageText))
        {
            res.AddError("Пустой текст сообщения");
            return res;
        }
        req.Payload.MessageText = req.Payload.MessageText.Trim();

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await helpdeskTransmissionRepo.IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
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
            await helpdeskTransmissionRepo.SubscribeUpdate(new()
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
                };

                await helpdeskTransmissionRepo.PulsePush(p_req);

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

                    string ReplaceTags(string raw)
                    {
                        return raw.Replace(GlobalStaticConstants.OrderDocumentName, order_obj.Name)
                        .Replace(GlobalStaticConstants.OrderDocumentDate, $"{order_obj.CreatedAtUTC.GetCustomTime().ToString("d", cultureInfo)} {order_obj.CreatedAtUTC.GetCustomTime().ToString("t", cultureInfo)}")
                        .Replace(GlobalStaticConstants.OrderStatusInfo, issue_data.StepIssue.DescriptionInfo())
                        .Replace(GlobalStaticConstants.OrderLinkAddress, $"<a href='{wc.Response?.ClearBaseUri}/issue-card/{order_obj.HelpdeskId}'>{_about_order}</a>")
                        .Replace(GlobalStaticConstants.HostAddress, $"<a href='{wc.Response?.ClearBaseUri}'>{wc.Response?.ClearBaseUri}</a>");
                    }

                    string subject_email = "Новое сообщение";
                    TResponseModel<string?> CommerceNewMessageOrderSubjectNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewMessageOrderSubjectNotification);
                    if (CommerceNewMessageOrderSubjectNotification.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderSubjectNotification.Response))
                        subject_email = CommerceNewMessageOrderSubjectNotification.Response;
                    subject_email = ReplaceTags(subject_email);

                    msg = $"<p>Заказ '{order_obj.Name}' от [{order_obj.CreatedAtUTC.GetCustomTime()}]: Новое сообщение.</p>" +
                                        $"<p>/<a href='{wc.Response?.ClearBaseUri}'>{wc.Response?.ClearBaseUri}</a>/</p>";

                    string tg_message = msg.Replace("<p>", "\n").Replace("</p>", "");

                    TResponseModel<string?> CommerceNewMessageOrderBodyNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewMessageOrderBodyNotification);
                    if (CommerceNewMessageOrderBodyNotification.Success() && !string.IsNullOrWhiteSpace(CommerceNewMessageOrderBodyNotification.Response))
                        msg = CommerceNewMessageOrderBodyNotification.Response;
                    msg = ReplaceTags(msg);

                    TResponseModel<string?> CommerceNewMessageOrderBodyNotificationTelegram = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewMessageOrderBodyNotificationTelegram);
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
                                TResponseModel<MessageComplexIdsModel?> tgs_res = await tgRepo.SendTextMessageTelegram(new()
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
                };

                await helpdeskTransmissionRepo.PulsePush(p_req);

                msg = "Сообщение успешно обновлено";
                res.AddSuccess(msg);
            }
        }

        return res;
    }
}