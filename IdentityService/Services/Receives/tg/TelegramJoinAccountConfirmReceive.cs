////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// TelegramJoinAccountConfirm receive
/// </summary>
public class TelegramJoinAccountConfirmReceive(IIdentityTools identityRepo, ILogger<TelegramJoinAccountConfirmReceive> _logger) 
    : IResponseReceive<TelegramJoinAccountConfirmModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountConfirmReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TelegramJoinAccountConfirmModel? confirm)
    {
        ArgumentNullException.ThrowIfNull(confirm);

        TResponseModel<object?> res = new();
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(confirm, GlobalStaticConstants.JsonSerializerSettings)}");
        string msg;
        if (confirm is null)
        {
            msg = $"remote call [payload] is null: error {{2AB259FB-AA62-4182-B463-9DE20110FDE9}}";
            res.AddError(msg);
            _logger.LogError(msg);
            return res;
        }

        return await identityRepo.TelegramJoinAccountConfirmTokenFromTelegram(confirm);
    }
}