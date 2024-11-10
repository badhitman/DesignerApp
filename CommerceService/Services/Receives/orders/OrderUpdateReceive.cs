////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using DbcLib;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrderUpdateReceive
/// </summary>
public class OrderUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory,
    IHttpClientFactory HttpClientFactory,
    WebConfigModel _webConf,
    ILogger<OrderUpdateReceive> loggerRepo,
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo,
    IWebRemoteTransmissionService webTransmissionRepo,
    ITelegramRemoteTransmissionService tgRepo,
    IHelpdeskRemoteTransmissionService hdRepo)
    : IResponseReceive<OrderDocumentModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrderUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(OrderDocumentModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        TResponseModel<UserInfoModel[]?> actor = await webTransmissionRepo.GetUsersIdentity([req.AuthorIdentityUserId]);

        if (!actor.Success() || actor.Response is null || actor.Response.Length == 0)
        {
            res.AddRangeMessages(actor.Messages);
            return res;
        }
        string msg, waMsg;
        DateTime dtu = DateTime.UtcNow;
        req.CreatedAtUTC = dtu;
        req.LastAtUpdatedUTC = dtu;

        req.PrepareForSave();
        if (req.Id < 1)
        {
            TResponseModel<int?> res_RubricIssueForCreateOrder = await StorageTransmissionRepo.ReadParameter<int?>(GlobalStaticConstants.CloudStorageMetadata.RubricIssueForCreateOrder);

            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction();
            try
            {
                req.StatusDocument = StatusesDocumentsEnum.Created;
                await context.AddAsync(req);
                await context.SaveChangesAsync();
                res.Response = req.Id;

                TAuthRequestModel<IssueUpdateRequestModel> issue_new = new()
                {
                    SenderActionUserId = req.AuthorIdentityUserId,
                    Payload = new()
                    {
                        Name = req.Name,
                        RubricId = res_RubricIssueForCreateOrder.Response,
                        Description = $"Новый заказ.\n{req.Information}".Trim(),
                    },
                };

                TResponseModel<int> issue = await hdRepo.IssueCreateOrUpdate(issue_new);

                if (!issue.Success())
                {
                    await transaction.RollbackAsync();
                    res.Messages.AddRange(issue.Messages);
                    return res;
                }

                req.HelpdeskId = issue.Response;
                context.Update(req);
                await context.SaveChangesAsync();

                transaction.Commit();
                CultureInfo cultureInfo = new("ru-RU");

                if (string.IsNullOrWhiteSpace(_webConf.ClearBaseUri))
                {
                    TResponseModel<TelegramBotConfigModel?> wc = await webTransmissionRepo.GetWebConfig();
                    _webConf.BaseUri = wc.Response?.ClearBaseUri;
                }

                string subject_email = "Создан новый заказ";
                TResponseModel<string?> CommerceNewOrderSubjectNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderSubjectNotification);
                if (CommerceNewOrderSubjectNotification.Success() && !string.IsNullOrWhiteSpace(CommerceNewOrderSubjectNotification.Response))
                    subject_email = CommerceNewOrderSubjectNotification.Response;

                DateTime _dt = DateTime.UtcNow.GetCustomTime();
                string _dtAsString = $"{_dt.ToString("d", cultureInfo)} {_dt.ToString("t", cultureInfo)}";

                string _about_order = $"'{req.Name}' {_dtAsString}";
                string ReplaceTags(string raw, bool clearMd = false)
                {
                    return raw.Replace(GlobalStaticConstants.OrderDocumentName, req.Name)
                    .Replace(GlobalStaticConstants.OrderDocumentDate, $"{_dtAsString}")
                    .Replace(GlobalStaticConstants.OrderStatusInfo, StatusesDocumentsEnum.Created.DescriptionInfo())
                    .Replace(GlobalStaticConstants.OrderLinkAddress, clearMd ? $"{_webConf.BaseUri}/issue-card/{req.HelpdeskId}" : $"<a href='{_webConf.BaseUri}/issue-card/{req.HelpdeskId}'>{_about_order}</a>")
                    .Replace(GlobalStaticConstants.HostAddress, clearMd ? _webConf.BaseUri : $"<a href='{_webConf.BaseUri}'>{_webConf.BaseUri}</a>");
                }

                subject_email = ReplaceTags(subject_email);

                res.AddSuccess(subject_email);
                msg = $"<p>Заказ <b>'{issue_new.Payload.Name}' от [{_dtAsString}]</b> успешно создан.</p>" +
                        $"<p>/<a href='{_webConf.ClearBaseUri}'>{_webConf.ClearBaseUri}</a>/</p>";
                string msg_for_tg = msg.Replace("<p>", "").Replace("</p>", "");

                waMsg = $"Заказ '{issue_new.Payload.Name}' от [{_dtAsString}] успешно создан.\n{_webConf.ClearBaseUri}";

                TResponseModel<string?> CommerceNewOrderBodyNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotification);
                if (CommerceNewOrderBodyNotification.Success() && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotification.Response))
                    msg = CommerceNewOrderBodyNotification.Response;
                msg = ReplaceTags(msg);

                TResponseModel<string?> CommerceNewOrderBodyNotificationTelegram = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotificationTelegram);
                if (CommerceNewOrderBodyNotificationTelegram.Success() && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotificationTelegram.Response))
                    msg_for_tg = CommerceNewOrderBodyNotificationTelegram.Response;
                msg_for_tg = ReplaceTags(msg_for_tg);

                List<Task> servicesCalls =
                [
                    webTransmissionRepo.SendEmail(new() { Email = actor.Response[0].Email!, Subject = subject_email, TextMessage = msg }),
                ];

                if (actor.Response[0].TelegramId.HasValue)
                    servicesCalls.Add(tgRepo.SendTextMessageTelegram(new() { Message = msg_for_tg, UserTelegramId = actor.Response[0].TelegramId!.Value }));

                if (!string.IsNullOrWhiteSpace(actor.Response[0].PhoneNumber) && GlobalTools.IsPhoneNumber(actor.Response[0].PhoneNumber!))
                {
                    TResponseModel<string?> wappiToken = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.WappiTokenApi);
                    TResponseModel<string?> wappiProfileId = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.WappiProfileId);

                    if (wappiToken.Success() && !string.IsNullOrWhiteSpace(wappiToken.Response) && wappiProfileId.Success() && !string.IsNullOrWhiteSpace(wappiProfileId.Response))
                    {
                        TResponseModel<string?> CommerceNewOrderBodyNotificationWhatsapp = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotificationWhatsapp);
                        if (CommerceNewOrderBodyNotificationWhatsapp.Success() && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotificationWhatsapp.Response))
                            waMsg = CommerceNewOrderBodyNotificationWhatsapp.Response;
                        waMsg = ReplaceTags(waMsg, true);

                        servicesCalls.Add(Task.Run(async () =>
                        {
                            using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Wappi.ToString());
                            if (!client.DefaultRequestHeaders.Any(x => x.Key == "Authorization"))
                                client.DefaultRequestHeaders.Add("Authorization", wappiToken.Response);

                            using HttpResponseMessage response = await client.PostAsJsonAsync($"/api/sync/message/send?profile_id={wappiProfileId.Response}", new SendMessageRequestModel() { Body = waMsg, Recipient = actor.Response[0].PhoneNumber! });
                            string rj = await response.Content.ReadAsStringAsync();
                            SendMessageResponseModel sendWappiRes = JsonConvert.DeserializeObject<SendMessageResponseModel>(rj)!;
                        }));
                    }
                }

                loggerRepo.LogInformation(msg_for_tg);
                await Task.WhenAll(servicesCalls);
                return res;
            }
            catch (Exception ex)
            {
                loggerRepo.LogError(ex, $"Не удалось создать заявку-заказ: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
                res.Messages.InjectException(ex);
                return res;
            }
        }

        OrderDocumentModelDB? order_document = await context.OrdersDocuments.FirstOrDefaultAsync(x => x.Id == req.Id);
        if (order_document is null)
        {
            res.AddError($"Документ #{req.Id} не найден");
            return res;
        }

        if (order_document.Name == req.Name && order_document.IsDisabled == req.IsDisabled)
        {
            res.AddInfo($"Документ #{req.Id} не требует обновления");
            return res;
        }

        res.Response = await context.OrdersDocuments
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, req.Name)
            .SetProperty(p => p.IsDisabled, req.IsDisabled)
            .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");

        return res;
    }
}