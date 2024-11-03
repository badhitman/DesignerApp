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
logger.Warn("init main");
IHostBuilder builder = Host.CreateDefaultBuilder(args);

// NLog: Setup NLog for Dependency injection
builder.ConfigureLogging((lc, lb) =>
{
    lb.ClearProviders();
    lb.AddNLog();
});
builder.UseNLog();

builder.ConfigureHostConfiguration(configHost =>
{
    string curr_dir = Directory.GetCurrentDirectory();
    configHost.SetBasePath(curr_dir);
    if (Path.Exists(Path.Combine(curr_dir, "appsettings.json")))
        configHost.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

#if DEBUG
    if (Path.Exists(Path.Combine(curr_dir, "appsettings.Development.json")))
        configHost.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
#else
    if (Path.Exists(Path.Combine(curr_dir, "appsettings.Production.json")))
        configHost.AddJsonFile($"appsettings.Production.json", optional: true, reloadOnChange: true);
#endif

    // Secrets
    string secretPath = Path.Combine("..", "secrets");
    for (int i = 0; i < 5 && !Directory.Exists(secretPath); i++)
        secretPath = Path.Combine("..", secretPath);
    if (Directory.Exists(secretPath))
        foreach (string secret in Directory.GetFiles(secretPath, $"*.json"))
            configHost.AddJsonFile(Path.GetFullPath(secret), optional: true, reloadOnChange: true);
    else
        logger.Warn("Секреты не найдены");

    configHost.AddEnvironmentVariables();
    configHost.AddCommandLine(args);
});

builder.ConfigureServices((context, services) =>
{
    services
    .Configure<RabbitMQConfigModel>(context.Configuration.GetSection("RabbitMQConfig"))
    .Configure<BotConfiguration>(context.Configuration.GetSection(BotConfiguration.Configuration))
    ;

    services.AddMemoryCache();
    services.AddSingleton<TelegramBotConfigModel>();
    services.AddOptions();


    string connectionIdentityString = context.Configuration.GetConnectionString("TelegramBotConnection") ?? throw new InvalidOperationException("Connection string 'HelpdeskConnection' not found.");
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