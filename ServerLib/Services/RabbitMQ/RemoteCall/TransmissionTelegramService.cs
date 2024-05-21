using Transmission.Receives.telegram;
using SharedLib;

namespace ServerLib;

/// <summary>
/// Удалённый вызов команд в TelegramBot службе
/// </summary>
public class TransmissionTelegramService(IRabbitClient rabbitClient) : ITelegramRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetBotUsername()
        => await rabbitClient.MqRemoteCall<string?>(typeof(GetBotUsernameReceive).FullName!);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> SendTextMessageTelegram(SendTextMessageTelegramBotModel message_telegram)
        => await rabbitClient.MqRemoteCall<int?>(typeof(SendTextMessageTelegramReceive).FullName!, message_telegram);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> SetWebConfig(WebConfigModel webConf)
        => await rabbitClient.MqRemoteCall<object?>(typeof(SetWebConfigReceive).FullName!, webConf);
}