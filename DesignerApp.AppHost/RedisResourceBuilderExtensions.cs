using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DesignerApp.AppHost
{
    internal static class RedisResourceBuilderExtensions
    {
        public static IResourceBuilder<RedisResource> WithClearCommand(
            this IResourceBuilder<RedisResource> builder)
        {
            builder.WithCommand(
                name: "clear-cache",
                displayName: "Clear Cache",
                executeCommand: context => OnRunClearCacheCommandAsync(builder, context),
                updateState: OnUpdateResourceState,
                iconName: "AnimalRabbitOff",
                iconVariant: IconVariant.Filled);

            return builder;
        }

        private static async Task<ExecuteCommandResult> OnRunClearCacheCommandAsync(
            IResourceBuilder<RedisResource> builder,
            ExecuteCommandContext context)
        {
            string connectionString = await builder.Resource.GetConnectionStringAsync() ??
                throw new InvalidOperationException(
                    $"Unable to get the '{context.ResourceName}' connection string.");

            await using var connection = ConnectionMultiplexer.Connect(connectionString);

            IDatabase database = connection.GetDatabase();

            await database.ExecuteAsync("FLUSHALL");

            return CommandResults.Success();
        }

        private static ResourceCommandState OnUpdateResourceState(
            UpdateCommandStateContext context)
        {
            ILogger<Program> logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Updating resource state: {ResourceSnapshot}",
                    context.ResourceSnapshot);
            }

            return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
                ? ResourceCommandState.Enabled
                : ResourceCommandState.Disabled;
        }
    }
}
