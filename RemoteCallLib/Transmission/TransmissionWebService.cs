////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// Удалённый вызов команд в Web службе
/// </summary>
public class TransmissionWebService(IRabbitClient rabbitClient) : IWebRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<WebConfigModel?>> GetWebConfig()
        => await rabbitClient.MqRemoteCall<WebConfigModel?>(GlobalStaticConstants.TransmissionQueues.GetWebConfigReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]?>> FindUsersIdentity(string[] ids_users)
        => await rabbitClient.MqRemoteCall<UserInfoModel[]?>(GlobalStaticConstants.TransmissionQueues.FindUsersOfIdentityReceive, ids_users);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> SendEmail(SendEmailRequestModel req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.SendEmailReceive, req);

    #region tg
    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountConfirmReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<CheckTelegramUserAuthModel?>> CheckTelegramUser(CheckTelegramUserHandleModel user)
        => await rabbitClient.MqRemoteCall<CheckTelegramUserAuthModel?>(GlobalStaticConstants.TransmissionQueues.UpdateTelegramUserReceive, user);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> TelegramJoinAccountDelete(long telegramId)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountDeleteReceive, telegramId);

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> UpdateTelegramMainUserMessage(MainUserMessageModel setMainMessage)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.UpdateTelegramMainUserMessageReceive, setMainMessage);

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModel?>> GetTelegramUser(long telegramUserId)
        => await rabbitClient.MqRemoteCall<TelegramUserBaseModel?>(GlobalStaticConstants.TransmissionQueues.GetTelegramUserReceive, telegramUserId);
    #endregion
}