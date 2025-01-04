////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AddressOrganizationUpdateReceive
/// </summary>
public class AddressOrganizationUpdateReceive(ICommerceService commerceRepo, ILogger<AddressOrganizationUpdateReceive> loggerRepo)
    : IResponseReceive<AddressOrganizationBaseModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddressOrganizationUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(AddressOrganizationBaseModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int> res = await commerceRepo.AddressOrganizationUpdate(req);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}