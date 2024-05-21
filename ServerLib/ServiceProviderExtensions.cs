using Microsoft.Extensions.Options;
using SharedLib;
using Telegram;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// ServiceProviderExtensions
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Получить конфигурацию
    /// </summary>
    public static T GetConfiguration<T>(this IServiceProvider serviceProvider)
        where T : class
    {
        IOptions<T>? o = serviceProvider.GetService<IOptions<T>>();
        return o is null ? throw new ArgumentNullException(nameof(T)) : o.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    public static IServiceCollection RegisterMqttListener<TQueue, TRequest, TResponse>(this IServiceCollection sc)
        where TQueue : class, IResponseReceive<TRequest?, TResponse?>
    {

        sc.AddScoped<IResponseReceive<TRequest?, TResponse?>, TQueue>();
        sc.AddHostedService<RabbitMqListenerService<TQueue, TRequest?, TResponse?>>();

        return sc;
    }
}
