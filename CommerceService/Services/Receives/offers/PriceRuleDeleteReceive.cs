////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// PriceRuleDeleteReceive
/// </summary>
public class PriceRuleDeleteReceive(ICommerceService commerceRepo, ILogger<PriceRuleDeleteReceive> loggerRepo)
    : IResponseReceive<TAuthRequestModel<int>?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PriceRuleDeleteCommerceReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TAuthRequestModel<int>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req.Payload, GlobalStaticConstants.JsonSerializerSettings)}");
        return await commerceRepo.PriceRuleDelete(req);
    }
}