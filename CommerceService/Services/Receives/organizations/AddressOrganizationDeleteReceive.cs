////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AddressOrganizationDeleteReceive
/// </summary>
public class AddressOrganizationDeleteReceive(ICommerceService commerceRepo, ILogger<AddressOrganizationDeleteReceive> loggerRepo)
    : IResponseReceive<int?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddressOrganizationDeleteCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(int? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        ResponseBaseModel res = await commerceRepo.AddressOrganizationDelete(req.Value);
        return new()
        {
            Messages = res.Messages,
        };
    }
}