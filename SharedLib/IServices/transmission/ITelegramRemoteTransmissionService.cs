////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Удалённый вызов команд в TelegramBot службе
/// </summary>
public interface ITelegramRemoteTransmissionService
{
    /// <summary>
    /// Получить Username для TelegramBot
    /// </summary>
    public Task<TResponseModel<string?>> GetBotUsername();

    /// <summary>
    /// Отправить сообщение через Telegram бота
    /// </summary>
    public Task<TResponseModel<int?>> SendTextMessageTelegram(SendTextMessageTelegramBotModel message_telegram);


    /// <summary>
    /// Установить WebConfig. От web части отправляется значение при загрузке браузера
    /// </summary>
    public Task<TResponseModel<object?>> SetWebConfig(WebConfigModel webConf);

    /// <summary>
    /// Прочитать данные по чатам
    /// </summary>
    /// <param name="chats_ids">Идентификаторы (Telegram) чатов</param>
    public Task<TResponseModel<ChatTelegramModelDB[]?>> ChatsReadTelegram(long[] chats_ids);

    /// <summary>
    /// Получить сообщения чата Telegram
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<MessageTelegramModelDB>?>> MessagesListTelegram(TPaginationRequestModel<int> req);
}