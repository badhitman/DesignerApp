using SharedLib;

namespace ServerLib;

/// <summary>
/// Удалённый вызов команд в Web службе
/// </summary>
public class TransmissionWebService(IRabbitClient rabbitClient) : IWebRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountConfirmReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<CheckTelegramUserModel?>> CheckTelegramUser(CheckTelegramUserHandleModel user)
        => await rabbitClient.MqRemoteCall<CheckTelegramUserModel?>(GlobalStaticConstants.TransmissionQueues.UpdateTelegramUserReceive, user);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> TelegramJoinAccountDelete(long telegramId)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountDeleteReceive, telegramId);

    /// <inheritdoc/>
    public async Task<TResponseModel<WebConfigModel?>> GetWebConfig()
        => await rabbitClient.MqRemoteCall<WebConfigModel?>(GlobalStaticConstants.TransmissionQueues.GetWebConfigReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> UpdateTelegramMainUserMessage(MainUserMessageModel setMainMessage)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.UpdateTelegramMainUserMessageReceive, setMainMessage);

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModelDb?>> GetTelegramUser(long telegramUserId)
        => await rabbitClient.MqRemoteCall<TelegramUserBaseModelDb?>(GlobalStaticConstants.TransmissionQueues.GetTelegramUserReceive, telegramUserId);
}