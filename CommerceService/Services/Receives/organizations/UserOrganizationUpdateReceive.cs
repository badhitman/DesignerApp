////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// UserOrganizationUpdateReceive
/// </summary>
public class UserOrganizationUpdateReceive(ICommerceService commerceRepo, ILogger<UserOrganizationUpdateReceive> loggerRepo)
    : IResponseReceive<TAuthRequestModel<UserOrganizationModelDB>?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationUserUpdateOrCreateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(TAuthRequestModel<UserOrganizationModelDB>? req)
    {
        ArgumentNullException.ThrowIfNull(req?.Payload);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<int> res = await commerceRepo.UserOrganizationUpdate(req);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages
        };
    }
}