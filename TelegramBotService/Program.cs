using Microsoft.EntityFrameworkCore;
using DbcLib;
using NLog;
using NLog.Extensions.Logging;
using SharedLib;
using Telegram.Bot;
using RemoteCallLib;
using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace TelegramBotService;

/// <summary>
/// Program
/// </summary>
public class Program
{
    /// <summary>
    /// Main
    /// </summary>
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        Logger logger = LogManager.GetCurrentClassLogger();
        builder.AddServiceDefaults();

        // NLog: Setup NLog for Dependency injection
        builder.Logging
            .ClearProviders()
            .AddNLog()
            .AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true; logging.IncludeScopes = true;
            });

        string _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Other";
        logger.Warn($"init main: {_environmentName}");
        string _modePrefix = Environment.GetEnvironmentVariable(nameof(GlobalStaticConstants.TransmissionQueueNamePrefix)) ?? "";
        if (!string.IsNullOrWhiteSpace(_modePrefix) && !GlobalStaticConstants.TransmissionQueueNamePrefix.EndsWith(_modePrefix))
            GlobalStaticConstants.TransmissionQueueNamePrefix += _modePrefix.Trim();
        string curr_dir = Directory.GetCurrentDirectory();


        builder.Configuration.SetBasePath(curr_dir);
        string path_load = Path.Combine(curr_dir, "appsettings.json");
        if (Path.Exists(path_load))
            builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);

        path_load = Path.Combine(curr_dir, $"appsettings.{_environmentName}.json");
        if (Path.Exists(path_load))
            builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
        else
            logger.Warn($"отсутствует: {path_load}");

        // Secrets
        void ReadSecrets(string dirName)
        {
            string secretPath = Path.Combine("..", dirName);
            DirectoryInfo di = new(secretPath);
            for (int i = 0; i < 5 && !di.Exists; i++)
            {
                logger.Warn($"файл секретов не найден (продолжение следует...): {di.FullName}");
                secretPath = Path.Combine("..", secretPath);
                di = new(secretPath);
            }

            if (Directory.Exists(secretPath))
            {
                foreach (string secret in Directory.GetFiles(secretPath, $"*.json"))
                {
                    path_load = Path.GetFullPath(secret);
                    logger.Warn($"!secret load: {path_load}");
                    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
                }
            }
            else
                logger.Warn($"—екреты `{dirName}` не найдены (совсем)");
        }
        ReadSecrets("secrets");
        if (!string.IsNullOrWhiteSpace(_modePrefix))
            ReadSecrets($"secrets{_modePrefix}");

        builder.Configuration.AddEnvironmentVariables();
        builder.Configuration.AddCommandLine(args);

        builder.Services
       .Configure<RabbitMQConfigModel>(builder.Configuration.GetSection(RabbitMQConfigModel.Configuration))
       .Configure<BotConfiguration>(builder.Configuration.GetSection(BotConfiguration.Configuration))
       ;
        builder.Services.AddHttpClient(HttpClientsNamesEnum.Wappi.ToString(), cc =>
        {
            cc.BaseAddress = new Uri("https://wappi.pro/");
        });
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<TelegramBotConfigModel>();
        builder.Services.AddOptions();

        string connectionIdentityString = builder.Configuration.GetConnectionString($"TelegramBotConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'TelegramBotConnection{_modePrefix}' not found.");
        builder.Services.AddDbContextFactory<TelegramBotContext>(opt =>
        {
            opt.UseNpgsql(connectionIdentityString);

#if DEBUG
            opt.EnableSensitiveDataLogging(true);
            opt.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
#endif
        });


        // Register named HttpClient to benefits from IHttpClientFactory
        // and consume it with ITelegramBotClient typed client.
        // More read:
        //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
        //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

        builder.Services
        .AddScoped<ITelegramBotService, TelegramBotServiceImplement>()
        .AddScoped<StoreTelegramService>()
        .AddScoped<UpdateHandler>()
        .AddScoped<ReceiverService>();

        builder.Services.AddHostedService<PollingService>();

        #region Telegram dialog - handlers answer to incoming messages
        builder.Services.AddScoped<ITelegramDialogService, DefaultTelegramDialogHandle>();
        #endregion

        #region MQ Transmission (remote methods call)
        string appName = typeof(Program).Assembly.GetName().Name ?? "AssemblyName";
        builder.Services.AddSingleton<IRabbitClient>(x =>
            new RabbitClient(x.GetRequiredService<IOptions<RabbitMQConfigModel>>(),
                        x.GetRequiredService<ILogger<RabbitClient>>(),
                        appName));

        builder.Services
        .AddScoped<IWebTransmission, WebTransmission>()
        .AddScoped<IHelpdeskTransmission, HelpdeskTransmission>()
        .AddScoped<IStorageTransmission, StorageTransmission>()
        .AddScoped<IIdentityTransmission, IdentityTransmission>()
        ;
        //
        builder.Services.TelegramBotRegisterMqListeners();
        #endregion

        // Custom metrics for the application
        Meter greeterMeter = new($"OTel.{appName}", "1.0.0");
        OpenTelemetryBuilder otel = builder.Services.AddOpenTelemetry();

        // Add Metrics for ASP.NET Core and our custom metrics and export via OTLP
        otel.WithMetrics(metrics =>
        {
            // Metrics provider from OpenTelemetry
            metrics.AddAspNetCoreInstrumentation();
            //Our custom metrics
            metrics.AddMeter(greeterMeter.Name);
            // Metrics provides by ASP.NET Core in .NET 8
            metrics.AddMeter("Microsoft.AspNetCore.Hosting");
        });

        // Add Tracing for ASP.NET Core and our custom ActivitySource and export via OTLP
        otel.WithTracing(tracing =>
        {
            tracing.AddSource($"OTel.{appName}");
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
        });

        IHost app = builder.Build();

        using (IServiceScope ss = app.Services.CreateScope())
        {
            TelegramBotConfigModel wc_main = ss.ServiceProvider.GetRequiredService<TelegramBotConfigModel>();
            IWebTransmission webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebTransmission>();
            TelegramBotConfigModel wc_remote = await webRemoteCall.GetWebConfig();
            wc_main.Update(wc_remote);
        }

        app.Run();
    }
}