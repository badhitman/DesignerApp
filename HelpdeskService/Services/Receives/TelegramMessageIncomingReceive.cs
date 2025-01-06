////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// TelegramMessageIncomingReceive
/// </summary>
public class TelegramMessageIncomingReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    ITelegramRemoteTransmissionService tgRepo,
    ISerializeStorageRemoteTransmissionService StorageRepo)
    : IResponseReceive<TelegramIncomingMessageModel?, TResponseModel<bool>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IncomingTelegramMessageHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>?> ResponseHandleAction(TelegramIncomingMessageModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool> res = new() { Response = false };
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

                TResponseModel<MessageComplexIdsModel?> send_answer = await tgRepo.SendTextMessageTelegram(sender);

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
                    send_answer = await tgRepo.SendTextMessageTelegram(sender);
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

                res.AddInfo("Сообщение является экспресс-ответом клиенту!");
                res.Response = true;
                return res;
            }
        }

        if (req.Chat?.Type != ChatsTypesTelegramEnum.Private)
            return res;

        TResponseModel<long?> helpdesk_user_redirect_telegram_for_issue_rest = await StorageRepo.ReadParameter<long?>(GlobalStaticConstants.CloudStorageMetadata.HelpdeskNotificationsTelegramForUser(req.From!.UserTelegramId));
        if (helpdesk_user_redirect_telegram_for_issue_rest.Success() && helpdesk_user_redirect_telegram_for_issue_rest.Response.HasValue && helpdesk_user_redirect_telegram_for_issue_rest.Response != 0)
        {
            TResponseModel<MessageComplexIdsModel?> forward_res = await tgRepo.ForwardMessage(new()
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
                res.AddSuccess($"Сообщение было переслано в чат #{helpdesk_user_redirect_telegram_for_issue_rest.Response.Value} по подписке на пользователя");
                return res;
            }
            else
                res.AddRangeMessages(forward_res.Messages);
        }

        helpdesk_user_redirect_telegram_for_issue_rest = await StorageRepo.ReadParameter<long?>(GlobalStaticConstants.CloudStorageMetadata.HelpdeskNotificationTelegramGlobalForIncomingMessage);
        if (helpdesk_user_redirect_telegram_for_issue_rest.Success() && helpdesk_user_redirect_telegram_for_issue_rest.Response.HasValue && helpdesk_user_redirect_telegram_for_issue_rest.Response != 0)
        {
            TResponseModel<MessageComplexIdsModel?> forward_res = await tgRepo.ForwardMessage(new()
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
                res.AddSuccess($"Сообщение было переслано в чат #{helpdesk_user_redirect_telegram_for_issue_rest.Response.Value} по глобальной подписке");
                return res;
            }
            else
                res.AddRangeMessages(forward_res.Messages);
        }

        return res;
    }

    async Task<List<FileAttachModel>?> Files(TelegramIncomingMessageModel req)
    {
        List<FileAttachModel> files = [];
        TResponseModel<byte[]?> data_res;
        //
        if (req.Audio is not null)
        {
            data_res = await tgRepo.GetFile(req.Audio.FileId);
            if (data_res.Success() && data_res.Response is not null && data_res.Response.Length != 0)
                files.Add(new() { ContentType = req.Audio.MimeType ?? "application/octet-stream", Data = data_res.Response, Name = req.Audio.FileName ?? req.Audio.Title ?? "Audio" });
        }
        if (req.Document is not null)
        {
            data_res = await tgRepo.GetFile(req.Document.FileId);
            if (data_res.Success() && data_res.Response is not null && data_res.Response.Length != 0)
                files.Add(new() { ContentType = req.Document.MimeType ?? "application/octet-stream", Data = data_res.Response, Name = req.Document.FileName ?? "Document" });
        }
        if (req.Photo is not null && req.Photo.Count != 0)
        {
            PhotoMessageTelegramModelDB _f = req.Photo.OrderByDescending(x => x.FileSize).First();
            data_res = await tgRepo.GetFile(_f.FileId);
            if (data_res.Success() && data_res.Response is not null && data_res.Response.Length != 0)
                files.Add(new() { ContentType = "image/jpeg", Data = data_res.Response, Name = "Photo" });
        }
        if (req.Voice is not null)
        {
            data_res = await tgRepo.GetFile(req.Voice.FileId);
            if (data_res.Success() && data_res.Response is not null && data_res.Response.Length != 0)
                files.Add(new() { ContentType = req.Voice.MimeType ?? "application/octet-stream", Data = data_res.Response, Name = "Voice" });
        }
        if (req.Video is not null)
        {
            data_res = await tgRepo.GetFile(req.Video.FileId);
            if (data_res.Success() && data_res.Response is not null && data_res.Response.Length != 0)
                files.Add(new() { ContentType = req.Video.MimeType ?? "application/octet-stream", Data = data_res.Response, Name = req.Video.FileName ?? "Video" });
        }

        return files.Count == 0 ? null : files;
    }
}