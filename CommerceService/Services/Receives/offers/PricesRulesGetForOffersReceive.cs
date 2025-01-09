////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// PricesRulesGetForOffersReceive
/// </summary>
public class PricesRulesGetForOffersReceive(ICommerceService commerceRepo)
    : IResponseReceive<TAuthRequestModel<int[]>?, TResponseModel<List<PriceRuleForOfferModelDB>>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PricesRulesGetForOfferCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<List<PriceRuleForOfferModelDB>>?> ResponseHandleAction(TAuthRequestModel<int[]>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commerceRepo.PricesRulesGetForOffers(req);
    }
}