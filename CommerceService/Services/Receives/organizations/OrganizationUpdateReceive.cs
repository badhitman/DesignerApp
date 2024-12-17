////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Organization update or create
/// </summary>
public class OrganizationUpdateReceive(ICommerceService commerceRepo, ILogger<OrganizationUpdateReceive> loggerRepo)
    : IResponseReceive<TAuthRequestModel<OrganizationModelDB>?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationUpdateOrCreateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(TAuthRequestModel<OrganizationModelDB>? req)
    {
        ArgumentNullException.ThrowIfNull(req?.Payload);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<int> res = await commerceRepo.OrganizationUpdate(req);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages
        };
    }
}