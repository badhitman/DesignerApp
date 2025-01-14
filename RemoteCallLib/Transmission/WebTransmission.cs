////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// Удалённый вызов команд в Web службе
/// </summary>
public class WebTransmission(IRabbitClient rabbitClient) : IWebTransmission
{
    /// <inheritdoc/>
    public async Task<TelegramBotConfigModel> GetWebConfig()
        => await rabbitClient.MqRemoteCall<TelegramBotConfigModel>(GlobalStaticConstants.TransmissionQueues.GetWebConfigReceive) ?? new();
}