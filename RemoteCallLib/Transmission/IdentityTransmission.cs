////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// [remote]: Identity
/// </summary>
public class IdentityTransmission(IRabbitClient rabbitClient) : IIdentityTransmission
{
    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> GetTwoFactorEnabled(string userId)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool?>>(GlobalStaticConstants.TransmissionQueues.GetTwoFactorEnabledReceive, userId) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetTwoFactorEnabled(SetTwoFactorEnabledRequestModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SetTwoFactorEnabledReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ResetAuthenticatorKey(string userId)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.ResetAuthenticatorKeyReceive, userId) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> RemoveLogin(RemoveLoginRequestModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.RemoveLoginForUserReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> VerifyTwoFactorToken(VerifyTwoFactorTokenRequestModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.VerifyTwoFactorTokenReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> CountRecoveryCodes(string userId)
        => await rabbitClient.MqRemoteCall<TResponseModel<int?>>(GlobalStaticConstants.TransmissionQueues.CountRecoveryCodesReceive, userId) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> GenerateChangeEmailToken(GenerateChangeEmailTokenRequestModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.GenerateChangeEmailTokenReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<IEnumerable<string>?>> GenerateNewTwoFactorRecoveryCodes(GenerateNewTwoFactorRecoveryCodesRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<IEnumerable<string>?>>(GlobalStaticConstants.TransmissionQueues.GenerateNewTwoFactorRecoveryCodesReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GetAuthenticatorKey(string userId)
        => await rabbitClient.MqRemoteCall<TResponseModel<string?>>(GlobalStaticConstants.TransmissionQueues.GetAuthenticatorKeyReceive, userId) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<string?>> GeneratePasswordResetTokenAsync(string userId)
        => await rabbitClient.MqRemoteCall<TResponseModel<string?>>(GlobalStaticConstants.TransmissionQueues.GeneratePasswordResetTokenReceive, userId) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SendPasswordResetLinkAsync(SendPasswordResetLinkRequestModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SendPasswordResetLinkReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ChangePassword(IdentityChangePasswordModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.ChangePasswordToUserReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddPassword(IdentityPasswordModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.AddPasswordToUserReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ChangeEmailAsync(IdentityEmailTokenModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.ChangeEmailForUserReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateUserDetails(IdentityDetailsModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.UpdateUserDetailsReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ClaimDelete(ClaimAreaIdModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.ClaimDeleteReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ClaimUpdateOrCreate(ClaimUpdateModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.ClaimUpdateOrCreateReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<List<ClaimBaseModel>> GetClaims(ClaimAreaOwnerModel req)
        => await rabbitClient.MqRemoteCall<List<ClaimBaseModel>>(GlobalStaticConstants.TransmissionQueues.GetClaimsReceive, req) ?? [];

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetLockUser(IdentityBooleanModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SetLockUserReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<UserInfoModel>> FindUsersAsync(FindWithOwnedRequestModel req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<UserInfoModel>>(GlobalStaticConstants.TransmissionQueues.FindUsersReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ResetPassword(IdentityPasswordTokenModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.ResetPasswordForUserReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel>> FindUserByEmail(string email)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserInfoModel>>(GlobalStaticConstants.TransmissionQueues.FindUserByEmailReceive, email) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> GenerateEmailConfirmation(SimpleUserIdentityModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.GenerateEmailConfirmationIdentityReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<RegistrationNewUserResponseModel> CreateNewUser(string userEmail)
        => await rabbitClient.MqRemoteCall<RegistrationNewUserResponseModel>(GlobalStaticConstants.TransmissionQueues.RegistrationNewUserReceive, userEmail) ?? new();

    /// <inheritdoc/>
    public async Task<RegistrationNewUserResponseModel> CreateNewUserWithPassword(RegisterNewUserPasswordModel req)
        => await rabbitClient.MqRemoteCall<RegistrationNewUserResponseModel>(GlobalStaticConstants.TransmissionQueues.RegistrationNewUserWithPasswordReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ConfirmUserEmailCode(UserCodeModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.ConfirmUserEmailCodeIdentityReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SendEmail(SendEmailRequestModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SendEmailReceive, req, waitResponse) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<UserInfoModel>> SelectUsersOfIdentity(TPaginationRequestModel<SimpleBaseRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<UserInfoModel>>(GlobalStaticConstants.TransmissionQueues.SelectUsersOfIdentityReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>> GetUsersIdentity(IEnumerable<string> ids_users)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserInfoModel[]>>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityReceive, ids_users) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ClaimsUserFlush(string userIdIdentity)
        => await rabbitClient.MqRemoteCall<TResponseModel<bool>>(GlobalStaticConstants.TransmissionQueues.ClaimsForUserFlushReceive, userIdIdentity) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByEmails(IEnumerable<string> ids_emails)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserInfoModel[]>>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByEmailReceive, ids_emails) ?? new();

    #region tg
    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>> GetUserIdentityByTelegram(long[] ids_users)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserInfoModel[]>>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByTelegramIdsReceive, ids_users) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByTelegram(List<long> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserInfoModel[]>>(GlobalStaticConstants.TransmissionQueues.GetUsersIdentityByTelegramReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramAccountRemoveIdentityJoin(TelegramAccountRemoveJoinRequestIdentityModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.TelegramAccountRemoveIdentityJoinReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountDeleteAction(string userId)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountDeleteActionReceive, userId) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountCreate(string userId)
        => await rabbitClient.MqRemoteCall<TResponseModel<TelegramJoinAccountModelDb>>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountCreateReceive, userId) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<TelegramUserViewModel>> FindUsersTelegram(FindRequestModel req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<TelegramUserViewModel>>(GlobalStaticConstants.TransmissionQueues.FindUsersTelegramReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountConfirmToken(TelegramJoinAccountConfirmModel req, bool waitResponse = true)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountConfirmReceive, req, waitResponse) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountDelete(TelegramAccountRemoveJoinRequestTelegramModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountDeleteReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateTelegramMainUserMessage(MainUserMessageModel setMainMessage)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.UpdateTelegramMainUserMessageReceive, setMainMessage) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModel>> GetTelegramUser(long telegramUserId)
        => await rabbitClient.MqRemoteCall<TResponseModel<TelegramUserBaseModel>>(GlobalStaticConstants.TransmissionQueues.GetTelegramUserReceive, telegramUserId) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountState(TelegramJoinAccountStateRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<TelegramJoinAccountModelDb>>(GlobalStaticConstants.TransmissionQueues.TelegramJoinAccountStateReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<CheckTelegramUserAuthModel>> CheckTelegramUser(CheckTelegramUserHandleModel user)
        => await rabbitClient.MqRemoteCall<TResponseModel<CheckTelegramUserAuthModel>>(GlobalStaticConstants.TransmissionQueues.CheckTelegramUserReceive, user) ?? new();
    #endregion

    #region roles
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TryAddRolesToUser(UserRolesModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.TryAddRolesToUserReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<RoleInfoModel>> GetRole(string roleName)
        => await rabbitClient.MqRemoteCall<TResponseModel<RoleInfoModel>>(GlobalStaticConstants.TransmissionQueues.GetRoleReceive, roleName) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<RoleInfoModel>> FindRolesAsync(FindWithOwnedRequestModel req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<RoleInfoModel>>(GlobalStaticConstants.TransmissionQueues.FindRolesAsyncReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CreateNewRole(string roleName)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.CateNewRoleReceive, roleName) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteRole(string roleName)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.DeleteRoleReceive, roleName) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteRoleFromUser(RoleEmailModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.DeleteRoleFromUserReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddRoleToUser(RoleEmailModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.AddRoleToUserReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<string[]>> SetRoleForUser(SetRoleForUserRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<string[]>>(GlobalStaticConstants.TransmissionQueues.SetRoleForUserOfIdentityReceive, req) ?? new();
    #endregion
}