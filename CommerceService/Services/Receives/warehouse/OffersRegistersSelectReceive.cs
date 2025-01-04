////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OffersRegistersSelectReceive
/// </summary>
public class OffersRegistersSelectReceive(ICommerceService commRepo)
: IResponseReceive<TPaginationRequestModel<RegistersSelectRequestBaseModel>?, TResponseModel<TPaginationResponseModel<OfferAvailabilityModelDB>>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OffersRegistersSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OfferAvailabilityModelDB>>?> ResponseHandleAction(TPaginationRequestModel<RegistersSelectRequestBaseModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commRepo.RegistersSelect(req);
    }
}