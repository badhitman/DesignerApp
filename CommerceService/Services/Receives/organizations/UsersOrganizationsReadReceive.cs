////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// UsersOrganizationsReadReceive
/// </summary>
public class UsersOrganizationsReadReceive(ICommerceService commerceRepo) 
    : IResponseReceive<int[], TResponseModel<UserOrganizationModelDB[]>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationsUsersReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<UserOrganizationModelDB[]>?> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commerceRepo.UsersOrganizationsRead(req);
    }
}