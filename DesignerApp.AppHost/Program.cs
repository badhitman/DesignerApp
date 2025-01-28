using Microsoft.Extensions.Configuration;
using SharedLib;

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
        if (!string.IsNullOrWhiteSpace(_modePrefix))
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

        IResourceBuilder<ProjectResource> apirestservice = builder.AddProject<Projects.ApiRestService>("apirestservice")
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            ;

        IResourceBuilder<ProjectResource> commerceservice = builder.AddProject<Projects.CommerceService>("commerceservice")
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            ;

        IResourceBuilder<ProjectResource> constructorservice = builder.AddProject<Projects.ConstructorService>("constructorservice")
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            ;

        IResourceBuilder<ProjectResource> helpdeskservice = builder.AddProject<Projects.HelpdeskService>("helpdeskservice")
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            ;

        IResourceBuilder<ProjectResource> identityservice = builder.AddProject<Projects.IdentityService>("identityservice")
            .WithEnvironment(act => smtpConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            ;

        IResourceBuilder<ProjectResource> storageservice = builder.AddProject<Projects.StorageService>("storageservice")
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            .WithEnvironment(act => mongoConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))
            ;

        IResourceBuilder<ProjectResource> telegramBot = builder.AddProject<Projects.TelegramBotService>("telegrambotpolling")
            .WithEnvironment(act => rabbitConfig.ForEach(x => act.EnvironmentVariables.Add(x.Key, x.Value ?? "")))

            ;

        builder.AddProject<Projects.BlankBlazorApp>("blankblazorapp")
            //.WithHttpEndpoint(port: 5066)
            .WithExternalHttpEndpoints()

            .WithReference(apirestservice)
            .WaitFor(apirestservice)
            .WithReference(commerceservice)
            .WaitFor(commerceservice)
            .WithReference(constructorservice)
            .WaitFor(constructorservice)
            .WithReference(helpdeskservice)
            .WaitFor(helpdeskservice)
            .WithReference(identityservice)
            .WaitFor(identityservice)
            .WithReference(storageservice)
            .WaitFor(storageservice)
            .WithReference(telegramBot)
            .WaitFor(telegramBot)
        ;

        builder.Build().Run();
    }
}