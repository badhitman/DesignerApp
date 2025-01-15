using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Extensions.Logging;
using Telegram.Bot.Services;
using RemoteCallLib;
using Telegram.Bot;
using SharedLib;
using NLog.Web;
using DbcLib;
using NLog;

// Early init of NLog to allow startup and exception logging, before host is built
Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

IHostBuilder builder = Host.CreateDefaultBuilder(args);

// NLog: Setup NLog for Dependency injection
builder.ConfigureLogging((lc, lb) =>
{
    lb.ClearProviders();
    lb.AddNLog();
});
builder.UseNLog();
string _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Other";
logger.Warn($"init main: {_environmentName}");

string _modePrefix = Environment.GetEnvironmentVariable(nameof(GlobalStaticConstants.TransmissionQueueNamePrefix)) ?? "";
if (!string.IsNullOrWhiteSpace(_modePrefix))
    GlobalStaticConstants.TransmissionQueueNamePrefix += _modePrefix.Trim();

string curr_dir = Directory.GetCurrentDirectory();
string path_load = Path.Combine(curr_dir, "appsettings.json");

builder.ConfigureHostConfiguration(configHost =>
{
    configHost.SetBasePath(curr_dir);

    if (Path.Exists(path_load))
        configHost.AddJsonFile(path_load, optional: true, reloadOnChange: true);
    else
        logger.Warn($"отсутствует: {path_load}");

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
            logger.Warn($"Секреты `{dirName}` не найдены (совсем)");
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
    .Configure<BotConfiguration>(context.Configuration.GetSection(BotConfiguration.Configuration))
    ;
    services.AddHttpClient(HttpClientsNamesEnum.Wappi.ToString(), cc =>
    {
        cc.BaseAddress = new Uri("https://wappi.pro/");
    });
    services.AddMemoryCache();
    services.AddSingleton<TelegramBotConfigModel>();
    services.AddOptions();

    string connectionIdentityString = context.Configuration.GetConnectionString($"TelegramBotConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'TelegramBotConnection{_modePrefix}' not found.");
    services.AddDbContextFactory<TelegramBotContext>(opt =>
    {
        opt.UseNpgsql(connectionIdentityString);

#if DEBUG
        opt.EnableSensitiveDataLogging(true);
        // opt.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
#endif
    });


    // Register named HttpClient to benefits from IHttpClientFactory
    // and consume it with ITelegramBotClient typed client.
    // More read:
    //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
    //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
    services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                TelegramBotClientOptions options = new(botConfig.BotToken);
                return new TelegramBotClient(options, httpClient);
            });

    // TelegramBotService : ITelegramBotService

    services
    .AddScoped<ITelegramBotService, TelegramBotService>()
    .AddScoped<StoreTelegramService>()
    .AddScoped<UpdateHandler>()
    .AddScoped<ReceiverService>();

    services.AddHostedService<PollingService>();

    #region Telegram dialog - handlers answer to incoming messages
    services.AddScoped<ITelegramDialogService, DefaultTelegramDialogHandle>();
    #endregion

    #region MQ Transmission (remote methods call)
    services
    .AddScoped<IRabbitClient, RabbitClient>()
    .AddScoped<IWebTransmission, WebTransmission>()
    .AddScoped<IHelpdeskTransmission, HelpdeskTransmission>()
    .AddScoped<IStorageTransmission, StorageTransmission>()
    .AddScoped<IIdentityTransmission, IdentityTransmission>()
    ;
    //
    services.TelegramBotRegisterMqListeners();
    #endregion
});

IHost app = builder.Build();

using (IServiceScope ss = app.Services.CreateScope())
{
    TelegramBotConfigModel wc_main = ss.ServiceProvider.GetRequiredService<TelegramBotConfigModel>();
    IWebTransmission webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebTransmission>();
    TelegramBotConfigModel wc_remote = await webRemoteCall.GetWebConfig();
    wc_main.Update(wc_remote);
}

await app.RunAsync();

#pragma warning disable CA1050 // Declare types in namespaces
/// <summary>
/// BotConfiguration
/// </summary>
public class BotConfiguration
#pragma warning restore CA1050 // Declare types in namespaces
{
    /// <inheritdoc/>
    public static readonly string Configuration = "BotConfiguration";

    /// <inheritdoc/>
    public string BotToken { get; set; } = "";
}