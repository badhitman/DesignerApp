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
    public async Task<TelegramBotConfigModel?> GetWebConfig()
        => await rabbitClient.MqRemoteCall<TelegramBotConfigModel?>(GlobalStaticConstants.TransmissionQueues.GetWebConfigReceive);

    /// <inheritdoc/>
    public async Task<UserInfoModel[]?> GetUsersIdentity(IEnumerable<string> ids_users)
        => await rabbitClient.MqRemoteCall<UserInfoModel[]?>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityReceive, ids_users);

    /// <inheritdoc/>
    public async Task<bool> SendEmail(SendEmailRequestModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.SendEmailReceive, req, waitResponse);

    #region tg
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountConfirmReceive, req, waitResponse);

    /// <inheritdoc/>
    public async Task<CheckTelegramUserAuthModel?> CheckTelegramUser(CheckTelegramUserHandleModel user)
        => await rabbitClient.MqRemoteCall<CheckTelegramUserAuthModel?>(GlobalStaticConstants.TransmissionQueues.UpdateTelegramUserReceive, user);

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountDelete(long telegramId)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountDeleteReceive, telegramId);

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateTelegramMainUserMessage(MainUserMessageModel setMainMessage)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.UpdateTelegramMainUserMessageReceive, setMainMessage);

    /// <inheritdoc/>
    public async Task<TelegramUserBaseModel?> GetTelegramUser(long telegramUserId)
        => await rabbitClient.MqRemoteCall<TelegramUserBaseModel?>(GlobalStaticConstants.TransmissionQueues.GetTelegramUserReceive, telegramUserId);

    /// <inheritdoc/>
    public async Task<UserInfoModel[]?> GetUserIdentityByTelegram(long[] ids_users)
        => await rabbitClient.MqRemoteCall<UserInfoModel[]?>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByTelegramIdsReceive, ids_users);

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<UserInfoModel>?> SelectUsersOfIdentity(TPaginationRequestModel<SimpleBaseRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<UserInfoModel>?>(GlobalStaticConstants.TransmissionQueues.SelectUsersOfIdentityReceive, req);

    /// <inheritdoc/>
    public async Task<string[]?> SetRoleForUser(SetRoleFoeUserRequestModel req)
        => await rabbitClient.MqRemoteCall<string[]?>(GlobalStaticConstants.TransmissionQueues.SetRoleForUserOfIdentityReceive, req);

    /// <inheritdoc/>
    public async Task<UserInfoModel[]?> GetUsersIdentityByEmails(IEnumerable<string> ids_emails)
        => await rabbitClient.MqRemoteCall<UserInfoModel[]?>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByEmailReceive, ids_emails);
    #endregion
}