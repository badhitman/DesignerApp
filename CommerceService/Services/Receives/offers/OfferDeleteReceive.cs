////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Удалить оффер
/// </summary>
public class OfferDeleteReceive(ICommerceService commerceRepo, ILogger<OfferDeleteReceive> loggerRepo)
    : IResponseReceive<TAuthRequestModel<int>?, ResponseBaseModel?>
{
    /// <summary>
    /// Удалить оффер
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferDeleteCommerceReceive;

    /// <summary>
    /// Удалить оффер
    /// </summary>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TAuthRequestModel<int>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        return await commerceRepo.OfferDelete(req);
    }
}