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
            .RegisterMqListener<GetWebConfigReceive, object, TelegramBotConfigModel>()           
            ;
    }
}