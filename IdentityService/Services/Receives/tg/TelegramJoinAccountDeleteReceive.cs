////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Удаление связи Telegram аккаунта с учётной записью сайта
/// </summary>
public class TelegramJoinAccountDeleteReceive(IIdentityTools identityRepo, ILogger<TelegramJoinAccountDeleteReceive> _logger) 
    : IResponseReceive<TelegramAccountRemoveJoinRequestTelegramModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountDeleteReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TelegramAccountRemoveJoinRequestTelegramModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(payload, GlobalStaticConstants.JsonSerializerSettings)}");
        string msg;
        if (payload.TelegramId == 0)
        {
            msg = $"remote call [payload] == default: error {{A3BF41B7-D330-42AF-A745-5E7C41E155DE}}";
            _logger.LogError(msg);
            return ResponseBaseModel.CreateError(msg);
        }

        try
        {
            ResponseBaseModel join_remove = await identityRepo.TelegramAccountRemoveTelegramJoin(payload);
            return ResponseBaseModel.Create(join_remove.Messages);
        }
        catch (Exception ex)
        {
            msg = "ошибка обработки входящего параметра `telegramUserId` .error C1722167-1ECE-430F-884D-2454C0494D7C";
            _logger.LogError(ex, msg);
            ResponseBaseModel res = ResponseBaseModel.CreateError(msg);
            res.Messages.InjectException(ex);
            return res;
        }
    }
}