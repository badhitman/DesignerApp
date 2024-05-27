using Microsoft.Extensions.Logging;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Update Telegram main user message
/// </summary>
public class UpdateTelegramMainUserMessageReceive(ITelegramWebService tgWebRepo, ILogger<UpdateTelegramMainUserMessageReceive> _logger)
    : IResponseReceive<MainUserMessageModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateTelegramMainUserMessageReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(MainUserMessageModel? setMainMessage)
    {
        TResponseModel<object?> res = new();
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
            ResponseBaseModel updMessage = await tgWebRepo.UpdateTelegramMainUserMessage(setMainMessage);
            res.AddRangeMessages(updMessage.Messages);
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