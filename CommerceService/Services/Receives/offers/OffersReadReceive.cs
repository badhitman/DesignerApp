////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OffersReadReceive
/// </summary>
public class OffersReadReceive(ICommerceService commerceRepo)
    : IResponseReceive<TAuthRequestModel<int[]>?, TResponseModel<OfferModelDB[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OfferModelDB[]>?> ResponseHandleAction(TAuthRequestModel<int[]>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commerceRepo.OffersRead(req);
    }
}