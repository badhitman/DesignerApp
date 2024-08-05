////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Удалённый вызов команд в Web службе
/// </summary>
public interface IWebRemoteTransmissionService
{
    /// <summary>
    /// Проверка Telegram пользователя
    /// </summary>
    public Task<TResponseModel<CheckTelegramUserModel?>> CheckTelegramUser(CheckTelegramUserHandleModel user);

    /// <summary>
    /// Проверка Telegram пользователя
    /// </summary>
    public Task<TResponseModel<object?>> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req);

    /// <summary>
    /// Удалить связь Telegram аккаунта с учётной записью сайта
    /// </summary>
    public Task<TResponseModel<object?>> TelegramJoinAccountDelete(long telegramId);

    /// <summary>
    /// Получить `web config` сайта
    /// </summary>
    public Task<TResponseModel<WebConfigModel?>> GetWebConfig();

    /// <summary>
    /// Основное сообщение в чате в котором Bot ведёт диалог с пользователем.
    /// Бот может отвечать новым сообщением или редактировать своё ранее отправленное в зависимости от ситуации.
    /// </summary>
    public Task<TResponseModel<object?>> UpdateTelegramMainUserMessage(MainUserMessageModel setMainMessage);

    /// <summary>
    /// Получить данные пользователя из кэша
    /// </summary>
    public Task<TResponseModel<TelegramUserBaseModelDb?>> GetTelegramUser(long telegramUserId);
}