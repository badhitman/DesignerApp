////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Получить информацию по пользователю (из БД).
/// Данные возвращаются из кэша: каждое сообщение в TelegramBot кеширует информацию о пользователе в БД
/// </summary>
public class GetTelegramUserReceive(IWebAppService tgWebRepo, ILogger<GetTelegramUserReceive> _logger)
    : IResponseReceive<long, TelegramUserBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetTelegramUserReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModel?>> ResponseHandleAction(long payload)
    {
        TResponseModel<TelegramUserBaseModel?> res = new();
        string msg;
        if (payload == 0)
        {
            msg = "remote call [payload] == default: error 76FC1696-0849-42F9-BFEA-57622031EB8F";
            _logger.LogError(msg);
            res.AddError(msg);
            return res;
        }

        try
        {
            res = await tgWebRepo.GetTelegramUserCachedInfo(payload);
        }
        catch (Exception ex)
        {
            msg = $"ошибка обработки запроса `{payload}`. error {{52351B07-1621-4F50-8F6A-CF97E39F3159}}";
            _logger.LogError(ex, msg);
            res.AddError(msg);
            res.Messages.InjectException(ex);
        }

        return res;
    }
}