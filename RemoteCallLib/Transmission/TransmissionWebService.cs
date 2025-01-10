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
    public async Task<TelegramBotConfigModel> GetWebConfig()
        => await rabbitClient.MqRemoteCall<TelegramBotConfigModel>(GlobalStaticConstants.TransmissionQueues.GetWebConfigReceive) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>> GetUsersIdentity(IEnumerable<string> ids_users)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserInfoModel[]>>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityReceive, ids_users) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SendEmail(SendEmailRequestModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SendEmailReceive, req, waitResponse) ?? new();

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

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<UserInfoModel>> SelectUsersOfIdentity(TPaginationRequestModel<SimpleBaseRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<UserInfoModel>>(GlobalStaticConstants.TransmissionQueues.SelectUsersOfIdentityReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<string[]>> SetRoleForUser(SetRoleFoeUserRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<string[]>>(GlobalStaticConstants.TransmissionQueues.SetRoleForUserOfIdentityReceive, req) ?? new();
    #endregion
}