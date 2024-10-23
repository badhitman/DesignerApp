////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using System.Globalization;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// StatusChangeReceive
/// </summary>
public class StatusChangeReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo,
    ILogger<StatusChangeReceive> LoggerRepo,
    ITelegramRemoteTransmissionService tgRepo,
    ICommerceRemoteTransmissionService commRepo,
    IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<StatusChangeRequestModel>?, bool>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.StatusChangeIssueHelpdeskReceive;
    static CultureInfo cultureInfo = new("ru-RU");
    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ResponseHandleAction(TAuthRequestModel<StatusChangeRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<bool> res = new()
        {
            Response = false,
        };

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await helpdeskTransmissionRepo.IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
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
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.StepIssue, req.Payload.Step));

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
            };

            await helpdeskTransmissionRepo.PulsePush(p_req);
            OrdersByIssuesSelectRequestModel req_docs = new()
            {
                IssueIds = [issue_data.Id],
            };

            TResponseModel<OrderDocumentModelDB[]> find_orders = await commRepo.OrdersByIssues(req_docs);
            if (find_orders.Success() && find_orders.Response is not null && find_orders.Response.Length != 0)
            {
                await commRepo.StatusOrderChange(new() { IssueId = issue_data.Id, Step = req.Payload.Step });
                TResponseModel<WebConfigModel?> wc = await webTransmissionRepo.GetWebConfig();
                OrderDocumentModelDB order_obj = find_orders.Response[0];
                string _about_order = $"'{order_obj.Name}' {order_obj.CreatedAtUTC.GetCustomTime().ToString("d", cultureInfo)} {order_obj.CreatedAtUTC.GetCustomTime().ToString("t", cultureInfo)}";
                DateTime cdd = order_obj.CreatedAtUTC.GetCustomTime();
                string ReplaceTags(string raw)
                {
                    return raw.Replace(GlobalStaticConstants.OrderDocumentName, order_obj.Name)
                    .Replace(GlobalStaticConstants.OrderDocumentDate, $"{cdd.ToString("d", cultureInfo)} {cdd.ToString("t", cultureInfo)}")
                    .Replace(GlobalStaticConstants.OrderStatusInfo, req.Payload.Step.DescriptionInfo())
                    .Replace(GlobalStaticConstants.OrderLinkAddress, $"<a href='{wc.Response?.ClearBaseUri}/issue-card/{order_obj.HelpdeskId}'>{_about_order}</a>")
                    .Replace(GlobalStaticConstants.HostAddress, $"<a href='{wc.Response?.ClearBaseUri}'>{wc.Response?.ClearBaseUri}</a>");
                }

                string subject_email = "Изменение статуса документа";
                TResponseModel<string?> CommerceStatusChangeOrderSubjectNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceStatusChangeOrderSubjectNotification(issue_data.StepIssue));
                if (CommerceStatusChangeOrderSubjectNotification.Success() && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderSubjectNotification.Response))
                    subject_email = CommerceStatusChangeOrderSubjectNotification.Response;
                subject_email = ReplaceTags(subject_email);

                msg = $"<p>Заказ '{order_obj.Name}' от [{order_obj.CreatedAtUTC.GetCustomTime()}] - {req.Payload.Step.DescriptionInfo()}.</p>" +
                                    $"<p>/<a href='{wc.Response?.ClearBaseUri}'>{wc.Response?.ClearBaseUri}</a>/</p>";

                string tg_message = msg.Replace("<p>", "\n").Replace("</p>", "");

                TResponseModel<string?> CommerceStatusChangeOrderBodyNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceStatusChangeOrderBodyNotification(issue_data.StepIssue));
                if (CommerceStatusChangeOrderBodyNotification.Success() && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderBodyNotification.Response))
                    msg = CommerceStatusChangeOrderBodyNotification.Response;
                msg = ReplaceTags(msg);

                TResponseModel<string?> CommerceStatusChangeOrderBodyNotificationTelegram = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceStatusChangeOrderBodyNotificationTelegram(issue_data.StepIssue));
                if (CommerceStatusChangeOrderBodyNotificationTelegram.Success() && !string.IsNullOrWhiteSpace(CommerceStatusChangeOrderBodyNotificationTelegram.Response))
                    tg_message = CommerceStatusChangeOrderBodyNotificationTelegram.Response;
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
                        LoggerRepo.LogInformation(tg_message.Replace("<b>", "").Replace("</b>", ""));
                        await webTransmissionRepo.SendEmail(new() { Email = u.Email!, Subject = subject_email, TextMessage = msg });
                    }
                }
            }
        }

        return res;
    }
}