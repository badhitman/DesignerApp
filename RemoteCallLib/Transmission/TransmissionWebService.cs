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
    public async Task<TResponseModel<TelegramBotConfigModel?>> GetWebConfig()
        => await rabbitClient.MqRemoteCall<TelegramBotConfigModel?>(GlobalStaticConstants.TransmissionQueues.GetWebConfigReceive);

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]?>> GetUsersIdentity(IEnumerable<string> ids_users)
        => await rabbitClient.MqRemoteCall<UserInfoModel[]?>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityReceive, ids_users);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> SendEmail(SendEmailRequestModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.SendEmailReceive, req, waitResponse);

    #region tg
    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<object?>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountConfirmReceive, req, waitResponse);

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

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]?>> GetUserIdentityByTelegram(long[] ids_users)
        => await rabbitClient.MqRemoteCall<UserInfoModel[]?>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByTelegramIdsReceive, ids_users);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<UserInfoModel>?>> SelectUsersOfIdentity(TPaginationRequestModel<SimpleBaseRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<UserInfoModel>?>(GlobalStaticConstants.TransmissionQueues.SelectUsersOfIdentityReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<string[]?>> SetRoleForUser(SetRoleFoeUserRequestModel req)
        => await rabbitClient.MqRemoteCall<string[]?>(GlobalStaticConstants.TransmissionQueues.SetRoleForUserOfIdentityReceive, req);
    #endregion
}