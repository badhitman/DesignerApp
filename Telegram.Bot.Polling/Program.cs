using Transmission.Receives.telegram;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Extensions.Logging;
using Telegram.Bot.Services;
using RemoteCallLib;
using Telegram.Bot;
using ServerLib;
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
string _modePrefix = "";
builder.ConfigureHostConfiguration(configHost =>
{
    ConfigurationBuilder bc = new();
    bc.AddCommandLine(args);
    IConfigurationRoot cb = bc.Build();
    _modePrefix = cb[nameof(GlobalStaticConstants.TransmissionQueueNamePrefix)] ?? "";
    if (!string.IsNullOrWhiteSpace(_modePrefix))
        GlobalStaticConstants.TransmissionQueueNamePrefix += _modePrefix.Trim();

    string curr_dir = Directory.GetCurrentDirectory();
    configHost.SetBasePath(curr_dir);
    string path_load = Path.Combine(curr_dir, "appsettings.json");
    if (Path.Exists(path_load))
        configHost.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    else
        logger.Warn($"отсутсвует: {path_load}");

#if DEBUG
    path_load = Path.Combine(curr_dir, "appsettings.Development.json");
    if (Path.Exists(path_load))
        configHost.AddJsonFile(path_load, optional: true, reloadOnChange: true);
    else
        logger.Warn($"отсутсвует: {path_load}");
#else
    path_load = Path.Combine(curr_dir, "appsettings.Production.json");
    if (Path.Exists(path_load))
        configHost.AddJsonFile(path_load, optional: true, reloadOnChange: true);
    else
        logger.Warn($"отсутсвует: {path_load}");
#endif

    // Secrets
    void ReadSecrets(string dirName)
    {
        string secretPath = Path.Combine("..", dirName);
        for (int i = 0; i < 5 && !Directory.Exists(secretPath); i++)
        {
            logger.Warn($"файл секретов не найден (продолжение следует...): {secretPath}");
            secretPath = Path.Combine("..", secretPath);
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
    logger.Warn($"init main: {context.HostingEnvironment.EnvironmentName}");
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

    services.AddScoped<StoreTelegramService>();
    services.AddScoped<UpdateHandler>();
    services.AddScoped<ReceiverService>();
    services.AddHostedService<PollingService>();

    #region Telegram dialog - handlers answer to incoming messages
    services.AddScoped<ITelegramDialogService, DefaultTelegramDialogHandle>();
    #endregion

    #region MQ Transmission (remote methods call)
    services
    .AddScoped<IRabbitClient, RabbitClient>()
    .AddScoped<IWebRemoteTransmissionService, TransmissionWebService>()
    .AddScoped<IHelpdeskRemoteTransmissionService, TransmissionHelpdeskService>()
    .AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>();
    //
    services.RegisterMqListener<SendTextMessageTelegramReceive, SendTextMessageTelegramBotModel, MessageComplexIdsModel?>()
    .RegisterMqListener<SetWebConfigReceive, TelegramBotConfigModel, object?>()
    .RegisterMqListener<GetBotUsernameReceive, object?, string?>()
    .RegisterMqListener<ChatsReadTelegramReceive, long[]?, ChatTelegramModelDB[]?>()
    .RegisterMqListener<MessagesSelectTelegramReceive, TPaginationRequestModel<SearchMessagesChatModel>?, TPaginationResponseModel<MessageTelegramModelDB>?>()
    .RegisterMqListener<GetFileTelegramReceive, string?, byte[]?>()
    .RegisterMqListener<SendWappiMessageReceive, EntryAltExtModel?, SendMessageResponseModel?>()
    .RegisterMqListener<ChatsFindForUserTelegramReceive, long[]?, ChatTelegramModelDB[]?>()
    .RegisterMqListener<ChatsSelectTelegramReceive, TPaginationRequestModel<string?>?, TPaginationResponseModel<ChatTelegramModelDB>?>()
    .RegisterMqListener<ForwardMessageTelegramReceive, ForwardMessageTelegramBotModel?, MessageComplexIdsModel?>()
    .RegisterMqListener<ChatTelegramReadReceive, int?, ChatTelegramModelDB?>()
    .RegisterMqListener<ErrorsForChatsSelectTelegramReceive, TPaginationRequestModel<long[]?>?, TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>?>();
    #endregion
});

IHost app = builder.Build();

using (IServiceScope ss = app.Services.CreateScope())
{
    TelegramBotConfigModel wc_main = ss.ServiceProvider.GetRequiredService<TelegramBotConfigModel>();
    IWebRemoteTransmissionService webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebRemoteTransmissionService>();
    TResponseModel<TelegramBotConfigModel?> wc_remote = await webRemoteCall.GetWebConfig();
    if (wc_remote.Response is not null && wc_remote.Success())
        wc_main.Update(wc_remote.Response);
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