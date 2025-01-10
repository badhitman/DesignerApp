////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;
using Transmission.Receives.Identity;

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
            ;
    }
}