////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// TelegramBot
/// </summary>
public interface ITelegramBotService
{
    /// <summary>
    /// ChatsFindForUserTelegram
    /// </summary>
    public Task<List<ChatTelegramModelDB>> ChatsFindForUserTelegram(long[] req);

    /// <summary>
    /// ChatsReadTelegram
    /// </summary>
    public Task<List<ChatTelegramModelDB>> ChatsReadTelegram(long[] req);

    /// <summary>
    /// ChatsSelectTelegram
    /// </summary>
    public Task<TPaginationResponseModel<ChatTelegramModelDB>> ChatsSelectTelegram(TPaginationRequestModel<string?> req);

    /// <summary>
    /// ChatTelegramRead
    /// </summary>
    public Task<ChatTelegramModelDB> ChatTelegramRead(int chatId);

    /// <summary>
    /// ErrorsForChatsSelectTelegram
    /// </summary>
    public Task<TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>> ErrorsForChatsSelectTelegram(TPaginationRequestModel<long[]> req);

    /// <summary>
    /// ForwardMessageTelegram
    /// </summary>
    public Task<TResponseModel<MessageComplexIdsModel>> ForwardMessageTelegram(ForwardMessageTelegramBotModel req);

    /// <summary>
    /// GetBotUsername
    /// </summary>
    public Task<TResponseModel<string>> GetBotUsername();

    /// <summary>
    /// GetFileTelegram
    /// </summary>
    public Task<TResponseModel<byte[]>> GetFileTelegram(string req);

    /// <summary>
    /// SendTextMessageTelegram
    /// </summary>
    public Task<TResponseModel<MessageComplexIdsModel>> SendTextMessageTelegram(SendTextMessageTelegramBotModel req);

    /// <summary>
    /// MessagesSelectTelegram
    /// </summary>
    public Task<TPaginationResponseModel<MessageTelegramModelDB>> MessagesSelectTelegram(TPaginationRequestModel<SearchMessagesChatModel> req);
}
