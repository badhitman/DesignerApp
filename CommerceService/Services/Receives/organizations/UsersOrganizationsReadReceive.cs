////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// UsersOrganizationsReadReceive
/// </summary>
public class UsersOrganizationsReadReceive(ICommerceService commerceRepo) : IResponseReceive<int[]?, UserOrganizationModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationsUsersReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<UserOrganizationModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<UserOrganizationModelDB[]> res = await commerceRepo.UsersOrganizationsRead(req);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}