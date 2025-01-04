////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Удаление связи Telegram аккаунта с учётной записью сайта
/// </summary>
public class TelegramJoinAccountDeleteReceive(IWebAppService tgWebRepo, ILogger<TelegramJoinAccountDeleteReceive> _logger)
    : IResponseReceive<long, ResponseBaseModel>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountDeleteReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(long payload)
    {
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(payload, GlobalStaticConstants.JsonSerializerSettings)}");
        string msg;
        if (payload == 0)
        {
            msg = $"remote call [payload] == default: error {{A3BF41B7-D330-42AF-A745-5E7C41E155DE}}";
            _logger.LogError(msg);
            return ResponseBaseModel.CreateError(msg);
        }

        try
        {
            ResponseBaseModel join_remove = await tgWebRepo.TelegramAccountRemoveJoin(payload);
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