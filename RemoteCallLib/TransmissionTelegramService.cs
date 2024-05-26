using SharedLib;

namespace ServerLib;

/// <summary>
/// Удалённый вызов команд в TelegramBot службе
/// </summary>
public class TransmissionTelegramService(IRabbitClient rabbitClient) : ITelegramRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetBotUsername()
        => await rabbitClient.MqRemoteCall<string?>(GlobalStaticConstants.TransmissionQueues.GetTelegramUserReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> SendTextMessageTelegram(SendTextMessageTelegramBotModel message_telegram)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.SendTextMessageTelegramReceive, message_telegram);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> SetWebConfig(WebConfigModel webConf)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.SetWebConfigReceive, webConf);
}