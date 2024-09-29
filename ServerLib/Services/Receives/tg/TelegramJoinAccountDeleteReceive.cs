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
    : IResponseReceive<long, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountDeleteReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(long payload)
    {
        TResponseModel<object?> res = new();
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(payload, GlobalStaticConstants.JsonSerializerSettings)}");
        string msg;
        if (payload == 0)
        {
            msg = $"remote call [payload] == default: error {{A3BF41B7-D330-42AF-A745-5E7C41E155DE}}";
            res.AddError(msg);
            _logger.LogError(msg);
            return res;
        }

        try
        {
            ResponseBaseModel join_remove = await tgWebRepo.TelegramAccountRemoveJoin(payload);
            res.AddRangeMessages(join_remove.Messages);
        }
        catch (Exception ex)
        {
            msg = "ошибка обработки входящего параметра `telegramUserId` .error C1722167-1ECE-430F-884D-2454C0494D7C";
            res.AddError(msg);
            _logger.LogError(ex, msg);
            res.Messages.InjectException(ex);
        }

        return res;
    }
}