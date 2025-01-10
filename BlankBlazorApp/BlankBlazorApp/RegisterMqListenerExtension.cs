////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Transmission.Receives.web;
using SharedLib;

namespace BlankBlazorApp;

/// <summary>
/// MQ listen
/// </summary>
public static class RegisterMqListenerExtension
{
    /// <summary>
    /// RegisterMqListeners
    /// </summary>
    public static IServiceCollection WebAppRegisterMqListeners(this IServiceCollection services)
    {
        return services
            .RegisterMqListener<UpdateTelegramUserReceive, CheckTelegramUserHandleModel, TResponseModel<CheckTelegramUserAuthModel?>>()
            .RegisterMqListener<TelegramJoinAccountConfirmReceive, TelegramJoinAccountConfirmModel, ResponseBaseModel>()
            .RegisterMqListener<TelegramJoinAccountDeleteReceive, long?, ResponseBaseModel>()
            .RegisterMqListener<GetWebConfigReceive, object, TelegramBotConfigModel>()
            .RegisterMqListener<UpdateTelegramMainUserMessageReceive, MainUserMessageModel, ResponseBaseModel>()
            .RegisterMqListener<GetTelegramUserReceive, long?, TResponseModel<TelegramUserBaseModel>>()
            .RegisterMqListener<GetUsersOfIdentityReceive, string[], TResponseModel<UserInfoModel[]>>()            
            .RegisterMqListener<SendEmailReceive, SendEmailRequestModel, ResponseBaseModel>()            
            .RegisterMqListener<SetRoleForUserReceive, SetRoleFoeUserRequestModel, TResponseModel<string[]>>()
            .RegisterMqListener<SelectUsersOfIdentityReceive, TPaginationRequestModel<SimpleBaseRequestModel>, TPaginationResponseModel<UserInfoModel>>()
            ;
    }
}