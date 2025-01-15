////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Update Telegram main user message
/// </summary>
public class UpdateTelegramMainUserMessageReceive(IIdentityTools identityRepo, ILogger<UpdateTelegramMainUserMessageReceive> _logger) : IResponseReceive<MainUserMessageModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateTelegramMainUserMessageReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(MainUserMessageModel? setMainMessage)
    {
        ArgumentNullException.ThrowIfNull(setMainMessage);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(setMainMessage, GlobalStaticConstants.JsonSerializerSettings)}");
        return await identityRepo.UpdateTelegramMainUserMessage(setMainMessage);
    }
}