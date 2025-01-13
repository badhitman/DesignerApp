////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// SelectUsersOfIdentityReceive
/// </summary>
public class SelectUsersOfIdentityReceive(IIdentityTools identityRepo)
    : IResponseReceive<TPaginationRequestModel<SimpleBaseRequestModel>?, TPaginationResponseModel<UserInfoModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SelectUsersOfIdentityReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<UserInfoModel>?> ResponseHandleAction(TPaginationRequestModel<SimpleBaseRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await identityRepo.SelectUsersOfIdentity(req);
    }
}