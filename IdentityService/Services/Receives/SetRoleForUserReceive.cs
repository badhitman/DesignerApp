////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// SetRoleForUserReceive
/// </summary>
public class SetRoleForUserReceive(IIdentityTools identityRepo, ILogger<SetRoleForUserReceive> _logger) 
    : IResponseReceive<SetRoleFoeUserRequestModel?, TResponseModel<string[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetRoleForUserOfIdentityReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<string[]>?> ResponseHandleAction(SetRoleFoeUserRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        return await identityRepo.SetRoleForUser(req);
    }
}