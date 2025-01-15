////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Получить информацию по пользователю (из БД).
/// Данные возвращаются из кэша: каждое сообщение в TelegramBot кеширует информацию о пользователе в БД
/// </summary>
public class GetTelegramUserReceive(IIdentityTools identityRepo, ILogger<GetTelegramUserReceive> _logger) : IResponseReceive<long?, TResponseModel<TelegramUserBaseModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetTelegramUserReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModel>?> ResponseHandleAction(long? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        TResponseModel<TelegramUserBaseModel> res = new();
        string msg;
        if (payload == 0)
        {
            msg = "remote call [payload] == default: error 76FC1696-0849-42F9-BFEA-57622031EB8F";
            _logger.LogError(msg);
            res.AddError(msg);
            return res;
        }

        return await identityRepo.GetTelegramUserCachedInfo(payload.Value);
    }
}