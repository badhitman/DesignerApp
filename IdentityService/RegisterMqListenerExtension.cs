////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using IdentityService.Services.Receives.users;
using Transmission.Receives.Identity;
using Transmission.Receives.web;
using SharedLib;

namespace IdentityService;

/// <summary>
/// MQ listen
/// </summary>
public static class RegisterMqListenerExtension
{
    /// <summary>
    /// RegisterMqListeners
    /// </summary>
    public static IServiceCollection IdentityRegisterMqListeners(this IServiceCollection services)
    {
        return services
            .RegisterMqListener<ClaimsUserFlushReceive, string, TResponseModel<bool>>()
            .RegisterMqListener<GetUsersIdentityByEmailReceive, string[], TResponseModel<UserInfoModel[]>>()
            .RegisterMqListener<GetUserIdentityByTelegramReceive, long[], TResponseModel<UserInfoModel[]>>()
            .RegisterMqListener<GetUsersOfIdentityReceive, string[], TResponseModel<UserInfoModel[]>>()
            .RegisterMqListener<SendEmailReceive, SendEmailRequestModel, ResponseBaseModel>()
            .RegisterMqListener<GenerateOTPFor2StepVerificationReceive, string, ResponseBaseModel>()
            .RegisterMqListener<GetUserLoginsReceive, string, TResponseModel<IEnumerable<UserLoginInfoModel>>>()
            .RegisterMqListener<CheckUserPasswordReceive, IdentityPasswordModel, ResponseBaseModel>()
            .RegisterMqListener<AddPasswordForUserReceive, IdentityPasswordModel, ResponseBaseModel>()
            .RegisterMqListener<UserHasPasswordReceive, string, TResponseModel<bool?>>()
            .RegisterMqListener<GetTwoFactorEnabledReceive, string, TResponseModel<bool?>>()
            .RegisterMqListener<SetTwoFactorEnabledReceive, SetTwoFactorEnabledRequestModel, ResponseBaseModel>()
            .RegisterMqListener<RemoveLoginReceive, RemoveLoginRequestModel, ResponseBaseModel>()
            .RegisterMqListener<GetRoleReceive, string, TResponseModel<RoleInfoModel>>()
            .RegisterMqListener<VerifyTwoFactorTokenReceive, VerifyTwoFactorTokenRequestModel, ResponseBaseModel>()
            .RegisterMqListener<CountRecoveryCodesReceive, string, TResponseModel<int?>>()
            .RegisterMqListener<GenerateNewTwoFactorRecoveryCodesReceive, GenerateNewTwoFactorRecoveryCodesRequestModel, TResponseModel<IEnumerable<string>?>>()
            .RegisterMqListener<GetAuthenticatorKeyReceive, string, TResponseModel<string?>>()
            .RegisterMqListener<GeneratePasswordResetTokenReceive, string, TResponseModel<string?>>()
            .RegisterMqListener<CheckTelegramUserReceive, CheckTelegramUserHandleModel, TResponseModel<CheckTelegramUserAuthModel>>()
            .RegisterMqListener<GetUsersIdentityByTelegramReceive, List<long>, TResponseModel<UserInfoModel[]>>()
            .RegisterMqListener<TelegramAccountRemoveIdentityJoinReceive, TelegramAccountRemoveJoinRequestIdentityModel, ResponseBaseModel>()
            .RegisterMqListener<TelegramJoinAccountCreateReceive, string, TResponseModel<TelegramJoinAccountModelDb>>()
            .RegisterMqListener<FindUsersTelegramReceive, FindRequestModel, TPaginationResponseModel<TelegramUserViewModel>>()
            .RegisterMqListener<TelegramJoinAccountStateReceive, TelegramJoinAccountStateRequestModel, TResponseModel<TelegramJoinAccountModelDb>>()
            .RegisterMqListener<ClaimUpdateOrCreateReceive, ClaimUpdateModel, ResponseBaseModel>()
            .RegisterMqListener<ClaimDeleteReceive, ClaimAreaIdModel, ResponseBaseModel>()
            .RegisterMqListener<TelegramJoinAccountDeleteActionReceive, string, ResponseBaseModel>()
            .RegisterMqListener<SendPasswordResetLinkReceive, SendPasswordResetLinkRequestModel, ResponseBaseModel>()
            .RegisterMqListener<TryAddRolesToUserReceive, UserRolesModel, ResponseBaseModel>()
            .RegisterMqListener<ChangePasswordForUserReceive, IdentityChangePasswordModel, ResponseBaseModel>()
            .RegisterMqListener<AddPasswordForUserReceive, IdentityPasswordModel, ResponseBaseModel>()
            .RegisterMqListener<ChangeEmailForUserReceive, IdentityEmailTokenModel, ResponseBaseModel>()
            .RegisterMqListener<UpdateUserDetailsReceive, IdentityDetailsModel, ResponseBaseModel>()
            .RegisterMqListener<GetClaimsReceive, ClaimAreaOwnerModel, List<ClaimBaseModel>>()
            .RegisterMqListener<SetLockUserReceive, IdentityBooleanModel, ResponseBaseModel>()
            .RegisterMqListener<FindUsersReceive, FindWithOwnedRequestModel, TPaginationResponseModel<UserInfoModel>>()
            .RegisterMqListener<FindRolesAsyncReceive, FindWithOwnedRequestModel, TPaginationResponseModel<RoleInfoModel>>()
            .RegisterMqListener<CateNewRoleReceive, string, ResponseBaseModel>()
            .RegisterMqListener<TelegramJoinAccountConfirmReceive, TelegramJoinAccountConfirmModel, ResponseBaseModel>()
            .RegisterMqListener<TelegramJoinAccountDeleteReceive, TelegramAccountRemoveJoinRequestTelegramModel, ResponseBaseModel>()
            .RegisterMqListener<UpdateTelegramMainUserMessageReceive, MainUserMessageModel, ResponseBaseModel>()
            .RegisterMqListener<GetTelegramUserReceive, long, TResponseModel<TelegramUserBaseModel>>()
            .RegisterMqListener<DeleteRoleReceive, string, ResponseBaseModel>()
            .RegisterMqListener<DeleteRoleFromUserReceive, RoleEmailModel, ResponseBaseModel>()
            .RegisterMqListener<AddRoleToUserReceive, RoleEmailModel, ResponseBaseModel>()
            .RegisterMqListener<ResetPasswordReceive, IdentityPasswordTokenModel, ResponseBaseModel>()
            .RegisterMqListener<FindUserByEmailReceive, string, TResponseModel<UserInfoModel>>()
            .RegisterMqListener<CreateNewUserReceive, string, RegistrationNewUserResponseModel>()
            .RegisterMqListener<CreateNewUserWithPasswordReceive, RegisterNewUserPasswordModel, RegistrationNewUserResponseModel>()
            .RegisterMqListener<ConfirmUserEmailCodeIdentityReceive, UserCodeModel, ResponseBaseModel>()
            .RegisterMqListener<SetRoleForUserReceive, SetRoleForUserRequestModel, TResponseModel<string[]>>()
            .RegisterMqListener<SelectUsersOfIdentityReceive, TPaginationRequestModel<SimpleBaseRequestModel>, TPaginationResponseModel<UserInfoModel>>()
            ;
    }
}