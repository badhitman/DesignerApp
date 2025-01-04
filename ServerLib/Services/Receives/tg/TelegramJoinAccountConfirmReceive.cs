////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// TelegramJoinAccountConfirm receive
/// </summary>
public class TelegramJoinAccountConfirmReceive(IWebAppService tgWebRepo, ILogger<TelegramJoinAccountConfirmReceive> _logger)
    : IResponseReceive<TelegramJoinAccountConfirmModel, ResponseBaseModel>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountConfirmReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TelegramJoinAccountConfirmModel? confirm)
    {
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

        try
        {
            ResponseBaseModel userCheck = await tgWebRepo.TelegramJoinAccountConfirmTokenFromTelegram(confirm);
            res.AddRangeMessages(userCheck.Messages);
        }
        catch (Exception ex)
        {
            msg = $"ошибка обработки запроса 354EFC73-B065-4F8C-AF00-808314C52E10";
            res.AddError(msg);
            _logger.LogError(ex, msg);
            res.Messages.InjectException(ex);
        }

        return res;
    }
}