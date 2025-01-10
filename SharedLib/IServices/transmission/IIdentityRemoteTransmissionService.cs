////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Identity
/// </summary>
public interface IIdentityRemoteTransmissionService
{
    /// <summary>
    /// Установить пользователю Claim`s[TelegramId, FirstName, LastName, PhoneNum]
    /// </summary>
    public Task<TResponseModel<bool>> ClaimsUserFlush(string userIdIdentity);

    /// <summary>
    /// Получить пользователей из Identity по их Email`s
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByEmails(IEnumerable<string> ids_emails);

    /// <summary>
    /// Поиск пользователей в Identity по их Telegram chat id
    /// </summary>
    public Task<TResponseModel<UserInfoModel[]>> GetUserIdentityByTelegram(long[] ids_users);

}
