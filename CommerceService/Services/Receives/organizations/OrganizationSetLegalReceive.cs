////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Organization set legal
/// </summary>
public class OrganizationSetLegalReceive(ICommerceService commerceRepo, ILogger<OrganizationSetLegalReceive> loggerRepo)
    : IResponseReceive<OrganizationLegalModel, TResponseModel<bool>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationSetLegalCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>?> ResponseHandleAction(OrganizationLegalModel? org)
    {
        ArgumentNullException.ThrowIfNull(org);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(org)}");
        return await commerceRepo.OrganizationSetLegal(org);
    }
}