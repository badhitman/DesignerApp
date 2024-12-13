using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazorWebLib;

/// <summary>
/// IdleCircuitHandlerServiceCollectionExtensions
/// </summary>
public static class IdleCircuitHandlerServiceCollectionExtensions
{
    /// <summary>
    /// AddIdleCircuitHandler
    /// </summary>
    public static IServiceCollection AddIdleCircuitHandler(
        this IServiceCollection services,
        Action<IdleCircuitOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddIdleCircuitHandler();

        return services;
    }

    /// <summary>
    /// AddIdleCircuitHandler
    /// </summary>
    public static IServiceCollection AddIdleCircuitHandler(
        this IServiceCollection services)
    {
        services.AddScoped<CircuitHandler, IdleCircuitHandler>();

        return services;
    }
}