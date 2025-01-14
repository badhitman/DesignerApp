////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Проверка пользователя (сообщение из службы TelegramBot серверной части сайта)
/// </summary>
public class CheckTelegramUserReceive(IIdentityTools identityRepo, ILogger<SendEmailReceive> _logger) 
    : IResponseReceive<CheckTelegramUserHandleModel?, TResponseModel<CheckTelegramUserAuthModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CheckTelegramUserReceive;

    /// <summary>
    /// Проверка пользователя (сообщение из службы TelegramBot серверной части сайта)
    /// </summary>
    public async Task<TResponseModel<CheckTelegramUserAuthModel>?> ResponseHandleAction(CheckTelegramUserHandleModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        return await identityRepo.CheckTelegramUser(req);
    }
}