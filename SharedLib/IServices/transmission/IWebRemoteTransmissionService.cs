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
    public Task<string[]?> SetRoleForUser(SetRoleFoeUserRequestModel req);

    /// <summary>
    /// Получить `web config` сайта
    /// </summary>
    public Task<TelegramBotConfigModel?> GetWebConfig();

    /// <summary>
    /// Получить пользователей из Identity по их идентификаторам
    /// </summary>
    public Task<UserInfoModel[]?> GetUsersIdentity(IEnumerable<string> ids_users);

    /// <summary>
    /// Получить пользователей из Identity по их Email`s
    /// </summary>
    public Task<UserInfoModel[]?> GetUsersIdentityByEmails(IEnumerable<string> ids_emails);

    /// <summary>
    /// Поиск пользователей в Identity по их Telegram chat id
    /// </summary>
    public Task<UserInfoModel[]?> GetUserIdentityByTelegram(long[] ids_users);

    /// <summary>
    /// SelectUsersOfIdentity
    /// </summary>
    public Task<TPaginationResponseModel<UserInfoModel>> SelectUsersOfIdentity(TPaginationRequestModel<SimpleBaseRequestModel> req);

    /// <summary>
    /// Отправка Email
    /// </summary>
    public Task<bool> SendEmail(SendEmailRequestModel req, bool waitResponse = true);

    #region tg
    /// <summary>
    /// Проверка Telegram пользователя
    /// </summary>
    public Task<CheckTelegramUserAuthModel?> CheckTelegramUser(CheckTelegramUserHandleModel user);

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
    public Task<TelegramUserBaseModel> GetTelegramUser(long telegramUserId);
    #endregion
}