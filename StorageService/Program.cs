using Microsoft.EntityFrameworkCore;
using Transmission.Receives.storage;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using MongoDB.Driver;
using StorageService;
using RemoteCallLib;
using SharedLib;
using NLog.Web;
using DbcLib;
using NLog;
using OpenTelemetry;
using System.Diagnostics.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace StorageService;

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

        string _environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Other";
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
            .Configure<WebConfigModel>(builder.Configuration.GetSection(WebConfigModel.Configuration))
            ;

        builder.Services.AddSingleton<WebConfigModel>();

        MongoConfigModel _jo = builder.Configuration.GetSection(MongoConfigModel.Configuration).Get<MongoConfigModel>()!;
        builder.Services.AddSingleton(new MongoClient(_jo.ToString()).GetDatabase(_jo.FilesSystemName));

        builder.Services.AddMemoryCache();

        builder.Services.AddOptions();

        string connectionStorage = builder.Configuration.GetConnectionString($"CloudParametersConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'CloudParametersConnection{_modePrefix}' not found.");
        builder.Services.AddDbContextFactory<StorageContext>(opt =>
            opt.UseNpgsql(connectionStorage));

        string connectionNlog = builder.Configuration.GetConnectionString($"NLogsConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'NLogsConnection{_modePrefix}' not found.");
        builder.Services.AddDbContextFactory<NLogsContext>(opt =>
            opt.UseNpgsql(connectionNlog));

        builder.Services
        .AddScoped<IHelpdeskTransmission, HelpdeskTransmission>()
            .AddScoped<ICommerceTransmission, CommerceTransmission>();

        #region MQ Transmission (remote methods call)
        string appName = typeof(Program).Assembly.GetName().Name ?? "AssemblyName";
        builder.Services
            .AddSingleton<IRabbitClient>(x => new RabbitClient(x.GetRequiredService<IOptions<RabbitMQConfigModel>>(), x.GetRequiredService<ILogger<RabbitClient>>(), appName));

        builder.Services
        .AddScoped<IWebTransmission, WebTransmission>()
            .AddScoped<ITelegramTransmission, TelegramTransmission>()
            .AddScoped<ISerializeStorage, StorageServiceImpl>()
            .AddScoped<IIdentityTransmission, IdentityTransmission>()
            ;
        //
        builder.Services.StorageRegisterMqListeners();
        //
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
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            tracing.AddSource($"OTel.{appName}");
        });


        IHost app = builder.Build();

        using (IServiceScope ss = app.Services.CreateScope())
        {
            WebConfigModel wc_main = ss.ServiceProvider.GetRequiredService<WebConfigModel>();
            IWebTransmission webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebTransmission>();
            TelegramBotConfigModel wc_remote = await webRemoteCall.GetWebConfig();
            if (Uri.TryCreate(wc_remote.BaseUri, UriKind.Absolute, out _))
                wc_main.Update(wc_remote.BaseUri);
        }

        await app.RunAsync();
    }
}