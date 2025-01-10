////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// Identity Service
/// </summary>
public class IdentityServiceTransmission(IRabbitClient rabbitClient) : IIdentityRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ClaimsUserFlush(string userIdIdentity)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.ClaimsForUserFlushReceive, userIdIdentity) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>> GetUserIdentityByTelegram(long[] ids_users)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserInfoModel[]>>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByTelegramIdsReceive, ids_users) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByEmails(IEnumerable<string> ids_emails)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserInfoModel[]>>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByEmailReceive, ids_emails) ?? new();
}
