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
    public async Task<TResponseModel<RoleInfoModel>> GetRole(string roleName)
        => await rabbitClient.MqRemoteCall<TResponseModel<RoleInfoModel>>(GlobalStaticConstants.TransmissionQueues.GetRoleReceive, roleName) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<UserInfoModel>> FindUsersAsync(FindWithOwnedRequestModel req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<UserInfoModel>>(GlobalStaticConstants.TransmissionQueues.FindUsersReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<RoleInfoModel>> FindRolesAsync(FindWithOwnedRequestModel req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<RoleInfoModel>>(GlobalStaticConstants.TransmissionQueues.FindRolesAsyncReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CateNewRole(string roleName)
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
    public async Task<ResponseBaseModel> ResetPassword(IdentityPasswordTokenModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.ResetPasswordForUserReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel>> FindUserByEmail(string email)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserInfoModel>>(GlobalStaticConstants.TransmissionQueues.GenerateEmailConfirmationIdentityReceive, email) ?? new();

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
    public async Task<TResponseModel<string[]>> SetRoleForUser(SetRoleFoeUserRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<string[]>>(GlobalStaticConstants.TransmissionQueues.SetRoleForUserOfIdentityReceive, req) ?? new();

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
    public async Task<TResponseModel<UserInfoModel[]>> GetUserIdentityByTelegram(long[] ids_users)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserInfoModel[]>>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByTelegramIdsReceive, ids_users) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>> GetUsersIdentityByEmails(IEnumerable<string> ids_emails)
        => await rabbitClient.MqRemoteCall<TResponseModel<UserInfoModel[]>>(GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByEmailReceive, ids_emails) ?? new();
}