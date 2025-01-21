////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// UsersOrganizationsSelectReceive
/// </summary>
public class UsersOrganizationsSelectReceive(ICommerceService commerceRepo) : IResponseReceive<TPaginationRequestAuthModel<UsersOrganizationsStatusesRequestModel>?, TResponseModel<TPaginationResponseModel<UserOrganizationModelDB>>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationsUsersSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<UserOrganizationModelDB>>?> ResponseHandleAction(TPaginationRequestAuthModel<UsersOrganizationsStatusesRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commerceRepo.UsersOrganizationsSelect(req);
    }
}