////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// Identity Service
/// </summary>
public class IdentityServiceTransmission(IRabbitClient rabbitClient) : IIdentityRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ClaimsUserFlush(string userIdIdentity)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.ClaimsForUserFlushReceive, userIdIdentity) ?? new();
}
