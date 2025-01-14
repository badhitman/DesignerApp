////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Удалённый вызов команд в Web службе
/// </summary>
public interface IWebTransmission
{
    /// <summary>
    /// Получить `web config` сайта
    /// </summary>
    public Task<TelegramBotConfigModel> GetWebConfig();

    #region tg
    /// <summary>
    /// Проверка Telegram пользователя
    /// </summary>
    public Task<ResponseBaseModel> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req, bool waitResponse = true);

    /// <summary>
    /// Удалить связь Telegram аккаунта с учётной записью сайта
    /// </summary>
    public Task<ResponseBaseModel> TelegramJoinAccountDelete(long telegramId);

    /// <summary>
    /// Основное сообщение в чате в котором Bot ведёт диалог с пользователем.
    /// Бот может отвечать новым сообщением или редактировать своё ранее отправленное в зависимости от ситуации.
    /// </summary>
    public Task<ResponseBaseModel> UpdateTelegramMainUserMessage(MainUserMessageModel setMainMessage);

    /// <summary>
    /// Получить данные пользователя из кэша
    /// </summary>
    public Task<TResponseModel<TelegramUserBaseModel>> GetTelegramUser(long telegramUserId);
    #endregion
}