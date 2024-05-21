using Transmission.Receives.web;
using SharedLib;

namespace ServerLib;

/// <summary>
/// Удалённый вызов команд в Web службе
/// </summary>
public class TransmissionWebService(IRabbitClient rabbitClient) : IWebRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req)
    => await rabbitClient.MqRemoteCall<object?>(typeof(TelegramJoinAccountConfirmReceive).FullName!, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<CheckTelegramUserModel?>> CheckTelegramUser(CheckTelegramUserHandleModel user)
        => await rabbitClient.MqRemoteCall<CheckTelegramUserModel?>(typeof(UpdateTelegramUserReceive).FullName!, user);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> TelegramJoinAccountDelete(long telegramId)
        => await rabbitClient.MqRemoteCall<object?>(typeof(TelegramJoinAccountDeleteReceive).FullName!, telegramId);

    /// <inheritdoc/>
    public async Task<TResponseModel<WebConfigModel?>> GetWebConfig()
        => await rabbitClient.MqRemoteCall<WebConfigModel?>(typeof(GetWebConfigReceive).FullName!);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> UpdateTelegramMainUserMessage(MainUserMessageModel setMainMessage)
        => await rabbitClient.MqRemoteCall<object?>(typeof(UpdateTelegramMainUserMessageReceive).FullName!, setMainMessage);

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModelDb?>> GetTelegramUser(long telegramUserId)
        => await rabbitClient.MqRemoteCall<TelegramUserBaseModelDb?>(typeof(GetTelegramUserReceive).FullName!, telegramUserId);
}