////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// UpdateTelegramUser
/// </summary>
public class UpdateTelegramUserReceive(ITelegramWebService tgWebRepo, ILogger<UpdateTelegramUserReceive> _logger)
    : IResponseReceive<CheckTelegramUserHandleModel?, CheckTelegramUserModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.UpdateTelegramUserReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<CheckTelegramUserModel?>> ResponseHandleAction(CheckTelegramUserHandleModel? user)
    {
        TResponseModel<CheckTelegramUserModel?> res = new();
        string msg;
        if (user is null)
        {
            msg = "remote call [user] is null: error 13AAA4B1-5417-483F-A0A7-F2DDAAC7B74F";
            _logger.LogError(msg);
            res.AddError(msg);
            return res;
        }

        try
        {
            res = await tgWebRepo.CheckTelegramUser(user);
        }
        catch (Exception ex)
        {
            msg = "ошибка обработки запроса. error F53277A8-26A2-49CB-B8EB-7DFC0AC7925D";
            _logger.LogError(ex, msg);
            res.AddError(msg);
            res.Messages.InjectException(ex);
        }

        return res;
    }
}