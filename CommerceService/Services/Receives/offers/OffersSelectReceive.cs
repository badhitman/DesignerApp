////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OffersSelectReceive
/// </summary>
public class OffersSelectReceive(ICommerceService commerceRepo)
    : IResponseReceive<TAuthRequestModel<TPaginationRequestModel<OffersSelectRequestModel>>?, TResponseModel<TPaginationResponseModel<OfferModelDB>>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OfferModelDB>>?> ResponseHandleAction(TAuthRequestModel<TPaginationRequestModel<OffersSelectRequestModel>>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commerceRepo.OffersSelect(req);
    }
}