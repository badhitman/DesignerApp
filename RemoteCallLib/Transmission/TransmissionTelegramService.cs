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
    public async Task<TResponseModel<ChatTelegramModelDB?>> ChatTelegramRead(int chatIdDb)
        => await rabbitClient.MqRemoteCall<ChatTelegramModelDB?>(GlobalStaticConstants.TransmissionQueues.ChatReadTelegramReceive, chatIdDb);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>?>> ErrorsForChatsSelectTelegram(TPaginationRequestModel<long[]?> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>?>(GlobalStaticConstants.TransmissionQueues.ErrorsForChatsSelectTelegramReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<MessageComplexIdsModel?>> ForwardMessage(ForwardMessageTelegramBotModel message)
        => await rabbitClient.MqRemoteCall<MessageComplexIdsModel?>(GlobalStaticConstants.TransmissionQueues.ForwardTextMessageTelegramReceive, message);

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetBotUsername()
        => await rabbitClient.MqRemoteCall<string?>(GlobalStaticConstants.TransmissionQueues.GetBotUsernameReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<byte[]?>> GetFile(string fileId)
        => await rabbitClient.MqRemoteCall<byte[]?>(GlobalStaticConstants.TransmissionQueues.ReadFileTelegramReceive, fileId);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<MessageTelegramModelDB>>> MessagesListTelegram(TPaginationRequestModel<SearchMessagesChatModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<MessageTelegramModelDB>>(GlobalStaticConstants.TransmissionQueues.MessagesChatsSelectTelegramReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<MessageComplexIdsModel?>> SendTextMessageTelegram(SendTextMessageTelegramBotModel message_telegram)
        => await rabbitClient.MqRemoteCall<MessageComplexIdsModel?>(GlobalStaticConstants.TransmissionQueues.SendTextMessageTelegramReceive, message_telegram);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> SetWebConfigHelpdesk(WebConfigModel webConf)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.SetWebConfigHelpdeskReceive, webConf);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> SetWebConfigStorage(WebConfigModel webConf)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.SetWebConfigStorageReceive, webConf);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> SetWebConfigTelegram(TelegramBotConfigModel webConf)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.SetWebConfigTelegramReceive, webConf);
}