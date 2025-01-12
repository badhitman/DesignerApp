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

    #region tg
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountConfirmReceive, req, waitResponse) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<CheckTelegramUserAuthModel?>> CheckTelegramUser(CheckTelegramUserHandleModel user)
        => await rabbitClient.MqRemoteCall<TResponseModel<CheckTelegramUserAuthModel?>>(GlobalStaticConstants.TransmissionQueues.UpdateTelegramUserReceive, user) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountDelete(long telegramId)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountDeleteReceive, telegramId) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateTelegramMainUserMessage(MainUserMessageModel setMainMessage)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.UpdateTelegramMainUserMessageReceive, setMainMessage) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModel>> GetTelegramUser(long telegramUserId)
        => await rabbitClient.MqRemoteCall<TResponseModel<TelegramUserBaseModel>>(GlobalStaticConstants.TransmissionQueues.GetTelegramUserReceive, telegramUserId) ?? new();
    #endregion
}