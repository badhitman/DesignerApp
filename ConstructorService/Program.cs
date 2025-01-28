////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Transmission.Receives.constructor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using ConstructorService;
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

builder.Services.AddOptions();

builder.Services
   .Configure<RabbitMQConfigModel>(builder.Configuration.GetSection(RabbitMQConfigModel.Configuration))
   .Configure<ConstructorConfigModel>(builder.Configuration.GetSection("ConstructorConfig"));

string connectionIdentityString = builder.Configuration.GetConnectionString($"ConstructorConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'ConstructorConnection{_modePrefix}' not found.");
builder.Services.AddDbContextFactory<ConstructorContext>(opt =>
{
    opt.UseNpgsql(connectionIdentityString);

#if DEBUG
    opt.EnableSensitiveDataLogging(true);
#endif
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IConstructorService, FormsConstructorService>();

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
builder.Services.ConstructorRegisterMqListeners();
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

using (IServiceScope ss = host.Services.CreateScope())
{
    IOptions<ConstructorConfigModel> wc_main = ss.ServiceProvider.GetRequiredService<IOptions<ConstructorConfigModel>>();
    IWebTransmission webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebTransmission>();
    wc_main.Value.WebConfig = await webRemoteCall.GetWebConfig();
}

host.Run();