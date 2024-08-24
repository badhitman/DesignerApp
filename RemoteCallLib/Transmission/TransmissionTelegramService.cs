////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// Удалённый вызов команд в TelegramBot службе
/// </summary>
public class TransmissionTelegramService(IRabbitClient rabbitClient) : ITelegramRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<ChatTelegramModelDB[]?>> ChatsFindForUser(long[] usersTelegramIds)
        => await rabbitClient.MqRemoteCall<ChatTelegramModelDB[]?>(GlobalStaticConstants.TransmissionQueues.ChatsFindForUserTelegramReceive, usersTelegramIds);

    /// <inheritdoc/>
    public async Task<TResponseModel<ChatTelegramModelDB[]?>> ChatsReadTelegram(long[] chats_ids)
        => await rabbitClient.MqRemoteCall<ChatTelegramModelDB[]?>(GlobalStaticConstants.TransmissionQueues.ChatsReadTelegramReceive, chats_ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<ChatTelegramModelDB>?>> ChatsSelect(TPaginationRequestModel<string?> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<ChatTelegramModelDB>?>(GlobalStaticConstants.TransmissionQueues.ChatsSelectTelegramReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ForwardMessage(ForwardMessageTelegramBotModel message)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.ForwardTextMessageTelegramReceive, message);

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetBotUsername()
        => await rabbitClient.MqRemoteCall<string?>(GlobalStaticConstants.TransmissionQueues.GetBotUsernameReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<byte[]?>> GetFile(string fileId)
        => await rabbitClient.MqRemoteCall<byte[]?>(GlobalStaticConstants.TransmissionQueues.ReadFileTelegramReceive, fileId);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<MessageTelegramModelDB>?>> MessagesListTelegram(TPaginationRequestModel<SearchMessagesChatModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<MessageTelegramModelDB>>(GlobalStaticConstants.TransmissionQueues.MessagesChatsSelectTelegramReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> SendTextMessageTelegram(SendTextMessageTelegramBotModel message_telegram)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.SendTextMessageTelegramReceive, message_telegram);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> SetWebConfig(WebConfigModel webConf)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.SetWebConfigReceive, webConf);
}