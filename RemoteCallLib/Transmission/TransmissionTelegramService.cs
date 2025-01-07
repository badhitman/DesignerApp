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
    public async Task<List<ChatTelegramModelDB>> ChatsFindForUser(long[] usersTelegramIds)
        => await rabbitClient.MqRemoteCall<List<ChatTelegramModelDB>>(GlobalStaticConstants.TransmissionQueues.ChatsFindForUserTelegramReceive, usersTelegramIds);

    /// <inheritdoc/>
    public async Task<List<ChatTelegramModelDB>> ChatsReadTelegram(long[] chats_ids)
        => await rabbitClient.MqRemoteCall<List<ChatTelegramModelDB>>(GlobalStaticConstants.TransmissionQueues.ChatsReadTelegramReceive, chats_ids);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<ChatTelegramModelDB>> ChatsSelect(TPaginationRequestModel<string?> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<ChatTelegramModelDB>>(GlobalStaticConstants.TransmissionQueues.ChatsSelectTelegramReceive, req);

    /// <inheritdoc/>
    public async Task<ChatTelegramModelDB> ChatTelegramRead(int chatIdDb)
        => await rabbitClient.MqRemoteCall<ChatTelegramModelDB>(GlobalStaticConstants.TransmissionQueues.ChatReadTelegramReceive, chatIdDb);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>> ErrorsForChatsSelectTelegram(TPaginationRequestModel<long[]?> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>>(GlobalStaticConstants.TransmissionQueues.ErrorsForChatsSelectTelegramReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<MessageComplexIdsModel>> ForwardMessage(ForwardMessageTelegramBotModel message, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<TResponseModel<MessageComplexIdsModel>>(GlobalStaticConstants.TransmissionQueues.ForwardTextMessageTelegramReceive, message, waitResponse);

    /// <inheritdoc/>
    public async Task<TResponseModel<string>> GetBotUsername()
        => await rabbitClient.MqRemoteCall<TResponseModel<string>>(GlobalStaticConstants.TransmissionQueues.GetBotUsernameReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<byte[]>> GetFile(string fileId)
        => await rabbitClient.MqRemoteCall<TResponseModel<byte[]>>(GlobalStaticConstants.TransmissionQueues.ReadFileTelegramReceive, fileId);

    /// <inheritdoc/>
    public async Task<TResponseModel<string>> GetTelegramBotToken()
        => await rabbitClient.MqRemoteCall<TResponseModel<string>>(GlobalStaticConstants.TransmissionQueues.GetBotTokenTelegramReceive);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<MessageTelegramModelDB>> MessagesListTelegram(TPaginationRequestModel<SearchMessagesChatModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<MessageTelegramModelDB>>(GlobalStaticConstants.TransmissionQueues.MessagesChatsSelectTelegramReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<MessageComplexIdsModel>> SendTextMessageTelegram(SendTextMessageTelegramBotModel message_telegram, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<TResponseModel<MessageComplexIdsModel>>(GlobalStaticConstants.TransmissionQueues.SendTextMessageTelegramReceive, message_telegram, waitResponse);

    /// <inheritdoc/>
    public async Task<TResponseModel<SendMessageResponseModel>> SendWappiMessage(EntryAltExtModel message, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<TResponseModel<SendMessageResponseModel>>(GlobalStaticConstants.TransmissionQueues.SendWappiMessageReceive, message, waitResponse);

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetWebConfigHelpdesk(WebConfigModel webConf, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SetWebConfigHelpdeskReceive, webConf, waitResponse);

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetWebConfigStorage(WebConfigModel webConf, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SetWebConfigStorageReceive, webConf, waitResponse);

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetWebConfigTelegram(TelegramBotConfigModel webConf, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SetWebConfigTelegramReceive, webConf, waitResponse);
}