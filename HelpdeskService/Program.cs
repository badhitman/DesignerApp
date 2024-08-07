using DbcLib;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using RemoteCallLib;
using SharedLib;
using Transmission.Receives.helpdesk;

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
    ;

    services.AddSingleton<WebConfigModel>();
    services.AddOptions();

    string connectionIdentityString = context.Configuration.GetConnectionString("HelpdeskConnection") ?? throw new InvalidOperationException("Connection string 'HelpdeskConnection' not found.");
    services.AddDbContextFactory<HelpdeskContext>(opt =>
    opt.UseSqlite(connectionIdentityString));

    #region MQ Transmission (remote methods call)
    services.AddScoped<IRabbitClient, RabbitClient>();
    services.AddScoped<IWebRemoteTransmissionService, TransmissionWebService>();
    services.AddScoped<ITelegramRemoteTransmissionService, TransmissionTelegramService>();
    ////
    services.RegisterMqListener<GetThemesIssuesReceive, object?, IssueThemeHelpdeskModelDB[]?>();
    services.RegisterMqListener<CreateIssueThemeReceive, IssueThemeHelpdeskModelDB?, int?>();
    services.RegisterMqListener<GetIssuesForUserReceive, UserCrossIdsModel?, IssueHelpdeskModelDB[]?>();
    services.RegisterMqListener<CreateIssueReceive, IssueHelpdeskModelDB?, int?>();
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