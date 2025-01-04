////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Update Telegram main user message
/// </summary>
public class UpdateTelegramMainUserMessageReceive(IWebAppService tgWebRepo, ILogger<UpdateTelegramMainUserMessageReceive> _logger)
    : IResponseReceive<MainUserMessageModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateTelegramMainUserMessageReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(MainUserMessageModel? setMainMessage)
    {
        ArgumentNullException.ThrowIfNull(setMainMessage);

        ResponseBaseModel res = new();
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(setMainMessage, GlobalStaticConstants.JsonSerializerSettings)}");
        string msg;
        if (setMainMessage is null)
        {
            msg = "remote call [SetMainMessage] is null: error CB78AEEE-8E5D-4BB5-B0B1-DCCD76CBAE37";
            _logger.LogError(msg);
            res.AddError(msg);
            return res;
        }

        try
        {
            return await tgWebRepo.UpdateTelegramMainUserMessage(setMainMessage);
        }
        catch (Exception ex)
        {
            msg = "ошибка обработки запроса. error 685DBC27-A8B7-480C-9CF3-2F5E7781BA65";
            _logger.LogError(ex, msg);
            res.AddError(msg);
            res.Messages.InjectException(ex);
        }

        return res;
    }
}