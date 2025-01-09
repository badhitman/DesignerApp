////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// PaymentDocumentUpdateReceive
/// </summary>
public class PaymentDocumentUpdateReceive(ICommerceService commerceRepo, ILogger<PaymentDocumentUpdateReceive> loggerRepo)
    : IResponseReceive<TAuthRequestModel<PaymentDocumentBaseModel>?, TResponseModel<int>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PaymentDocumentUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int>?> ResponseHandleAction(TAuthRequestModel<PaymentDocumentBaseModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        return await commerceRepo.PaymentDocumentUpdate(req);
    }
}