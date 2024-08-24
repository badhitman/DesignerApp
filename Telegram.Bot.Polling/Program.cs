using DbcLib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using RemoteCallLib;
using ServerLib;
using SharedLib;
using Telegram.Bot;
using Telegram.Bot.Services;
using Transmission.Receives.telegram;

// Early init of NLog to allow startup and exception logging, before host is built
Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
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

    services.AddSingleton<WebConfigModel>();
    services.AddOptions();


    string connectionIdentityString = context.Configuration.GetConnectionString("TelegramBotConnection") ?? throw new InvalidOperationException("Connection string 'HelpdeskConnection' not found.");
    services.AddDbContextFactory<TelegramBotContext>(opt =>
    {
        opt.UseSqlite(connectionIdentityString);

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
    services.AddScoped<IRabbitClient, RabbitClient>();
    services.AddScoped<IWebRemoteTransmissionService, TransmissionWebService>();
    services.AddScoped<IHelpdeskRemoteTransmissionService, TransmissionHelpdeskService>();
    services.AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>();
    //
    services.RegisterMqListener<SendTextMessageTelegramReceive, SendTextMessageTelegramBotModel, int?>();
    services.RegisterMqListener<SetWebConfigReceive, WebConfigModel, object?>();
    services.RegisterMqListener<GetBotUsernameReceive, object?, string?>();
    services.RegisterMqListener<ChatsReadTelegramReceive, long[]?, ChatTelegramModelDB[]?>();
    services.RegisterMqListener<MessagesSelectTelegramReceive, TPaginationRequestModel<SearchMessagesChatModel>?, TPaginationResponseModel<MessageTelegramModelDB>?>();
    services.RegisterMqListener<GetFileTelegramReceive, string?, byte[]?>();
    services.RegisterMqListener<ChatsFindForUserTelegramReceive, long[]?, ChatTelegramModelDB[]?>();
    services.RegisterMqListener<ChatsSelectTelegramReceive, TPaginationRequestModel<string?>?, TPaginationResponseModel<ChatTelegramModelDB>?>();
    services.RegisterMqListener<ForwardMessageTelegramReceive, ForwardMessageTelegramBotModel?, int?>();
    #endregion
});

IHost app = builder.Build();

using (IServiceScope ss = app.Services.CreateScope())
{
    WebConfigModel wc_main = ss.ServiceProvider.GetRequiredService<WebConfigModel>();
    IWebRemoteTransmissionService webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebRemoteTransmissionService>();
    TResponseModel<WebConfigModel?> wc_remote = await webRemoteCall.GetWebConfig();
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