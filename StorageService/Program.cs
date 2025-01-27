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

// Early init of NLog to allow startup and exception logging, before host is built
Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

IHostBuilder builder = Host.CreateDefaultBuilder(args);

// NLog: Setup NLog for Dependency injection
builder.ConfigureLogging((lc, lb) =>
{
    lb.ClearProviders()
    .AddNLog()
    .AddOpenTelemetry(logging =>
    {
        logging.IncludeFormattedMessage = true; logging.IncludeScopes = true;
    });
});
builder.UseNLog();
string _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Other";
logger.Warn($"init main: {_environmentName}");
string _modePrefix = Environment.GetEnvironmentVariable(nameof(GlobalStaticConstants.TransmissionQueueNamePrefix)) ?? "";
if (!string.IsNullOrWhiteSpace(_modePrefix))
    GlobalStaticConstants.TransmissionQueueNamePrefix += _modePrefix.Trim();

string curr_dir = Directory.GetCurrentDirectory();
builder.ConfigureHostConfiguration(configHost =>
{
    configHost.SetBasePath(curr_dir);
    string path_load = Path.Combine(curr_dir, "appsettings.json");
    if (Path.Exists(path_load))
        configHost.AddJsonFile(path_load, optional: true, reloadOnChange: true);

    path_load = Path.Combine(curr_dir, $"appsettings.{_environmentName}.json");
    if (Path.Exists(path_load))
        configHost.AddJsonFile(path_load, optional: true, reloadOnChange: true);
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
                configHost.AddJsonFile(path_load, optional: true, reloadOnChange: true);
            }
        }
        else
            logger.Warn($"—екреты `{dirName}` не найдены (совсем)");
    }
    ReadSecrets("secrets");
    if (!string.IsNullOrWhiteSpace(_modePrefix))
        ReadSecrets($"secrets{_modePrefix}");

    configHost.AddEnvironmentVariables();
    configHost.AddCommandLine(args);
});

builder.ConfigureServices((context, services) =>
{
    services
    .Configure<RabbitMQConfigModel>(context.Configuration.GetSection("RabbitMQConfig"))
    .Configure<WebConfigModel>(context.Configuration.GetSection("WebConfig"))
    ;
    MongoConfigModel _jo = context.Configuration.GetSection("MongoDB").Get<MongoConfigModel>()!;
    services.AddSingleton(new MongoClient(_jo.ToString()).GetDatabase(_jo.FilesSystemName));

    services.AddMemoryCache();
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = context.Configuration.GetConnectionString($"RedisConnectionString{_modePrefix}");
    });
    services.AddSingleton<IManualCustomCacheService, ManualCustomCacheService>();

    services.AddOptions();

    string connectionIdentityString = context.Configuration.GetConnectionString($"CloudParametersConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'CloudParametersConnection{_modePrefix}' not found.");
    services.AddDbContextFactory<StorageContext>(opt =>
    opt.UseNpgsql(connectionIdentityString));

    services
    .AddScoped<IHelpdeskTransmission, HelpdeskTransmission>()
    .AddScoped<ICommerceTransmission, CommerceTransmission>();

    #region MQ Transmission (remote methods call)
    string appName = typeof(Program).Assembly.GetName().Name ?? "AssemblyName";
    services.AddSingleton<IRabbitClient>(x =>
        new RabbitClient(x.GetRequiredService<IOptions<RabbitMQConfigModel>>(),
                    x.GetRequiredService<ILogger<RabbitClient>>(),
                    appName));

    services
    .AddScoped<IWebTransmission, WebTransmission>()
    .AddScoped<ITelegramTransmission, TelegramTransmission>()
    .AddScoped<ISerializeStorage, StorageServiceImpl>()
    .AddScoped<IIdentityTransmission, IdentityTransmission>()
    ;
    //
    services.StorageRegisterMqListeners();
    //
    #endregion
    
    // Custom metrics for the application
    Meter greeterMeter = new($"OTel.{appName}", "1.0.0");
    OpenTelemetryBuilder otel = services.AddOpenTelemetry();

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
});

IHost app = builder.Build();

using (IServiceScope ss = app.Services.CreateScope())
{
    IOptions<WebConfigModel> wc_main = ss.ServiceProvider.GetRequiredService<IOptions<WebConfigModel>>();
    IWebTransmission webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebTransmission>();
    TelegramBotConfigModel wc_remote = await webRemoteCall.GetWebConfig();
    if (Uri.TryCreate(wc_remote.BaseUri, UriKind.Absolute, out _))
        wc_main.Value.Update(wc_remote.BaseUri);
}

await app.RunAsync();