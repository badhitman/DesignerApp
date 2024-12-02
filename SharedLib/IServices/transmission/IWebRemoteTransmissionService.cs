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
    /// Получить `web config` сайта
    /// </summary>
    public Task<TResponseModel<string[]?>> SetRoleForUser(SetRoleFoeUserRequestModel req);

    /// <summary>
    /// Получить `web config` сайта
    /// </summary>
    public Task<TResponseModel<TelegramBotConfigModel?>> GetWebConfig();

    /// <summary>
    /// Получить пользователей из Identity по их идентификаторам
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]?>> GetUsersIdentity(IEnumerable<string> ids_users);

    /// <summary>
    /// Поиск пользователей в Identity по их Telegram chat id
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]?>> GetUserIdentityByTelegram(long[] ids_users);

    /// <summary>
    /// SelectUsersOfIdentity
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<UserInfoModel>?>> SelectUsersOfIdentity(TPaginationRequestModel<SimpleBaseRequestModel> req);

    /// <summary>
    /// Отправка Email
    /// </summary>
    public Task<TResponseModel<bool>> SendEmail(SendEmailRequestModel req, bool waitResponse = true);

    #region tg
    /// <summary>
    /// Проверка Telegram пользователя
    /// </summary>
    public Task<TResponseModel<CheckTelegramUserAuthModel?>> CheckTelegramUser(CheckTelegramUserHandleModel user);

    /// <summary>
    /// Проверка Telegram пользователя
    /// </summary>
    public Task<TResponseModel<object?>> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req, bool waitResponse = true);

    /// <summary>
    /// Удалить связь Telegram аккаунта с учётной записью сайта
    /// </summary>
    public Task<TResponseModel<object?>> TelegramJoinAccountDelete(long telegramId);

    /// <summary>
    /// Основное сообщение в чате в котором Bot ведёт диалог с пользователем.
    /// Бот может отвечать новым сообщением или редактировать своё ранее отправленное в зависимости от ситуации.
    /// </summary>
    public Task<TResponseModel<object?>> UpdateTelegramMainUserMessage(MainUserMessageModel setMainMessage);

    /// <summary>
    /// Получить данные пользователя из кэша
    /// </summary>
    public Task<TResponseModel<TelegramUserBaseModel?>> GetTelegramUser(long telegramUserId);
    #endregion
}