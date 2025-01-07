////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrganizationsReadReceive
/// </summary>
public class OrganizationsReadReceive(ICommerceService commerceRepo) : IResponseReceive<int[]?, TResponseModel<OrganizationModelDB[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationsReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OrganizationModelDB[]>?> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commerceRepo.OrganizationsRead(req);
    }
}