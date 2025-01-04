////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AddressesOrganizationsReadReceive
/// </summary>
public class AddressesOrganizationsReadReceive(ICommerceService commerceRepo)
    : IResponseReceive<int[], TResponseModel<AddressOrganizationModelDB[]>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddressesOrganizationsReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<AddressOrganizationModelDB[]>?> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commerceRepo.AddressesOrganizationsRead(req);
    }
}