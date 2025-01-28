////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Transmission.Receives.commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Extensions.Logging;
using CommerceService;
using RemoteCallLib;
using SharedLib;
using NLog.Web;
using DbcLib;
using NLog;
using OpenTelemetry;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Extensions.Options;

// Early init of NLog to allow startup and exception logging, before host is built
Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder
    .Logging
    .ClearProviders()
    .AddNLog()
    .AddOpenTelemetry(logging =>
    {
        logging.IncludeFormattedMessage = true;
        logging.IncludeScopes = true;
    });

string _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? builder.Environment.EnvironmentName;
logger.Warn($"init main: {_environmentName}");

string _modePrefix = Environment.GetEnvironmentVariable(nameof(GlobalStaticConstants.TransmissionQueueNamePrefix)) ?? "";
if (!string.IsNullOrWhiteSpace(_modePrefix))
    GlobalStaticConstants.TransmissionQueueNamePrefix += _modePrefix.Trim();

logger.Warn($"Префикс рабочего контура/контекста: {(string.IsNullOrWhiteSpace(_modePrefix) ? "НЕ ИСПОЛЬЗУЕТСЯ" : $"`{_modePrefix}`")}");

string curr_dir = Directory.GetCurrentDirectory();
builder.Configuration.SetBasePath(curr_dir);

builder.Configuration.SetBasePath(curr_dir);
string path_load = Path.Combine(curr_dir, "appsettings.json");
if (Path.Exists(path_load))
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
else
    logger.Warn($"отсутствует: {path_load}");

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
        logger.Warn($"Секреты `{dirName}` не найдены (совсем)");
}
ReadSecrets("secrets");
if (!string.IsNullOrWhiteSpace(_modePrefix))
    ReadSecrets($"secrets{_modePrefix}");

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

builder.Services
.Configure<RabbitMQConfigModel>(builder.Configuration.GetSection(RabbitMQConfigModel.Configuration))
;

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString($"RedisConnectionString{_modePrefix}");
    // options.InstanceName = "app.";
});

builder.Services.AddScoped<ICommerceService, CommerceImplementService>();

builder.Services.AddSingleton<WebConfigModel>();
builder.Services.AddOptions();
builder.Services.AddSingleton<IManualCustomCacheService, ManualCustomCacheService>();
string connectionIdentityString = builder.Configuration.GetConnectionString($"CommerceConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'CommerceConnection{_modePrefix}' not found.");
builder.Services.AddDbContextFactory<CommerceContext>(opt =>
{
    opt.UseNpgsql(connectionIdentityString);

#if DEBUG
    opt.EnableSensitiveDataLogging(true);
    opt.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
#endif
});

builder.Services.AddMemoryCache();

#region MQ Transmission (remote methods call)
string appName = typeof(Program).Assembly.GetName().Name ?? "AssemblyName";
builder.Services.AddSingleton<IRabbitClient>(x =>
    new RabbitClient(x.GetRequiredService<IOptions<RabbitMQConfigModel>>(),
                x.GetRequiredService<ILogger<RabbitClient>>(),
                appName));
//
builder.Services.AddScoped<IWebTransmission, WebTransmission>()
.AddScoped<ITelegramTransmission, TelegramTransmission>()
.AddScoped<IHelpdeskTransmission, HelpdeskTransmission>()
.AddScoped<IStorageTransmission, StorageTransmission>()
.AddScoped<IIdentityTransmission, IdentityTransmission>()
;
//
builder.Services.CommerceRegisterMqListeners();
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
IHost host = builder.Build();

host.Run();