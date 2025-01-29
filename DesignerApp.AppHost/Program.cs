using Aspire.Hosting;
using Microsoft.Extensions.Configuration;
using SharedLib;
using StackExchange.Redis;

namespace DesignerApp.AppHost;

/// <summary>
/// Program
/// </summary>
public class Program
{
    /// <summary>
    /// Main
    /// </summary>
    public static void Main(string[] args)
    {
        IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

        string _modePrefix = Environment.GetEnvironmentVariable(nameof(GlobalStaticConstants.TransmissionQueueNamePrefix)) ?? "";
        if (!string.IsNullOrWhiteSpace(_modePrefix) && !GlobalStaticConstants.TransmissionQueueNamePrefix.EndsWith(_modePrefix))
            GlobalStaticConstants.TransmissionQueueNamePrefix += _modePrefix.Trim();

        string curr_dir = Directory.GetCurrentDirectory();
        builder.Configuration.SetBasePath(curr_dir);
        string path_load = Path.Combine(curr_dir, "appsettings.json");
        if (Path.Exists(path_load))
            builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);

        string _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? builder.Environment.EnvironmentName;
        path_load = Path.Combine(curr_dir, $"appsettings.{_environmentName}.json");
        if (Path.Exists(path_load))
            builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);

        void ReadSecrets(string dirName)
        {
            string secretPath = Path.Combine("..", dirName);
            DirectoryInfo di = new(secretPath);
            for (int i = 0; i < 5 && !di.Exists; i++)
            {
                secretPath = Path.Combine("..", secretPath);
                di = new(secretPath);
            }

            if (Directory.Exists(secretPath))
            {
                foreach (string secret in Directory.GetFiles(secretPath, $"*.json"))
                {
                    path_load = Path.GetFullPath(secret);
                    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
                }
            }
        }

        ReadSecrets($"secrets-{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}");
        if (!string.IsNullOrWhiteSpace(_modePrefix))
            ReadSecrets($"secrets{_modePrefix}");

        builder.Configuration.AddEnvironmentVariables();
        builder.Configuration.AddCommandLine(args);

        List<KeyValuePair<string, string?>> smtpConfig = builder.Configuration
            .GetChildren()
            .Where(x => x.Path.Equals(SmtpConfigModel.Configuration, StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.AsEnumerable())
            .ToList();

        List<KeyValuePair<string, string?>> rabbitConfig = builder.Configuration
            .GetChildren()
            .Where(x => x.Path.Equals(RabbitMQConfigModel.Configuration, StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.AsEnumerable())
            .ToList();

        List<KeyValuePair<string, string?>> mongoConfig = builder.Configuration
            .GetChildren()
            .Where(x => x.Path.Equals(MongoConfigModel.Configuration, StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.AsEnumerable())
            .ToList();

        List<KeyValuePair<string, string?>> botTelegramConfig = builder.Configuration
            .GetChildren()
            .Where(x => x.Path.Equals(BotConfiguration.Configuration, StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.AsEnumerable())
            .ToList();

        /*
        //IResourceBuilder<RabbitMQServerResource> rabbit = builder.AddRabbitMQ("rabbit")
        //    //.WithImageTag("latest")
        //    //.WithLifetime(ContainerLifetime.Persistent)
        //    .WithManagementPlugin()
        //    .WithBindMount("VolumeMount.AppHost-rabbit-data", "/var/lib/rabbitmq");

        //IResourceBuilder<RedisResource> cache = builder.AddRedis("cache")
        //    .WithImageTag("latest")
        //    //.WithLifetime(ContainerLifetime.Persistent)
        //    .WithClearCommand()
        //    .WithBindMount("VolumeMount.AppHost-redis-data", "/data");

        //IResourceBuilder<MongoDBServerResource> mongo = builder.AddMongoDB("mongo")
        //    .WithImageTag("latest")
        //    .WithMongoExpress()
        //    //.WithLifetime(ContainerLifetime.Persistent)
        //    .WithBindMount("VolumeMount.AppHost-mongo-data", "/data/db");

        //IResourceBuilder<PostgresServerResource> postgress = builder.AddPostgres("postgress")
        //    .WithImageTag("latest")
        //    //.WithLifetime(ContainerLifetime.Persistent)
        //    .WithPgAdmin()
        //    .WithPgWeb()
        //    .WithBindMount("VolumeMount.AppHost-postgress-data", "/var/lib/postgresql/data");

        //IResourceBuilder<ParameterResource> envWithAspire = builder.AddParameter("WithAspire");

        //IResourceBuilder<ProjectResource> InfrastructureWait(IResourceBuilder<ProjectResource> sender)
        //{
        //    return sender.WithReference(cache)
        //    .WaitFor(cache)
        //    .WithReference(mongo)
        //    .WaitFor(mongo)
        //    .WithReference(rabbit)
        //    .WaitFor(rabbit)
        //    .WithReference(postgress)
        //    .WaitFor(postgress)

        //    .WithEnvironment(GlobalStaticConstants.AspireOrchestration, envWithAspire);
        //}
        */

        IResourceBuilder<IResourceWithConnectionString> redisConnectionStr = builder.AddConnectionString($"RedisConnectionString{_modePrefix}");
        IResourceBuilder<IResourceWithConnectionString> identityConnectionStr = builder.AddConnectionString($"IdentityConnection{_modePrefix}");

        IResourceBuilder<ProjectResource> helpdeskService = builder.AddProject<Projects.HelpdeskService>("helpdeskservice")
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            .WithReference(builder.AddConnectionString($"HelpdeskConnection{_modePrefix}"))
            .WithReference(redisConnectionStr)
            ;

        IResourceBuilder<ProjectResource> apiRestService = builder.AddProject<Projects.ApiRestService>("apirestservice")
            .WithReference(redisConnectionStr)
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            ;

        IResourceBuilder<ProjectResource> commerceService = builder.AddProject<Projects.CommerceService>("commerceservice")
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            .WithReference(builder.AddConnectionString($"CommerceConnection{_modePrefix}"))
            .WithReference(redisConnectionStr)
            ;

        IResourceBuilder<ProjectResource> constructorService = builder.AddProject<Projects.ConstructorService>("constructorservice")
            .WithReference(builder.AddConnectionString($"ConstructorConnection{_modePrefix}"))
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            ;

        IResourceBuilder<ProjectResource> identityService = builder.AddProject<Projects.IdentityService>("identityservice")
            .WithReference(identityConnectionStr)
            .WithReference(redisConnectionStr)
            .WithEnvironment(act => smtpConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            ;

        IResourceBuilder<ProjectResource> storageService = builder.AddProject<Projects.StorageService>("storageservice")
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            .WithEnvironment(act => mongoConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            .WithReference(builder.AddConnectionString($"CloudParametersConnection{_modePrefix}"))
            ;

        IResourceBuilder<ProjectResource> telegramBotService = builder.AddProject<Projects.TelegramBotService>("telegrambotpolling")
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            .WithEnvironment($"{BotConfiguration.Configuration}:{nameof(BotConfiguration.BotToken)}", builder.Configuration[$"{BotConfiguration.Configuration}:{nameof(BotConfiguration.BotToken)}"])
            .WithReference(builder.AddConnectionString($"TelegramBotConnection{_modePrefix}"))
            ;

        builder.AddProject<Projects.BlankBlazorApp>("blankblazorapp")
            .WithReference(helpdeskService)
            .WaitFor(helpdeskService)

            .WithReference(telegramBotService)
            .WaitFor(telegramBotService)

            .WithReference(identityService)
            .WaitFor(identityService)

            //.WithHttpEndpoint(port: 5066)
            .WithExternalHttpEndpoints()

            .WithReference(builder.AddConnectionString($"MainConnection{_modePrefix}"))
            .WithReference(identityConnectionStr)

            .WithReference(apiRestService)
            .WaitFor(apiRestService)
            .WithReference(commerceService)
            .WaitFor(commerceService)
            .WithReference(constructorService)
            .WaitFor(constructorService)

            .WithReference(storageService)
            .WaitFor(storageService)
        ;

        builder.Build().Run();
    }
}