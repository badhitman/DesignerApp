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
    {
        opt.UseSqlite(connectionIdentityString);

#if DEBUG
        opt.EnableSensitiveDataLogging(true);
#endif
    });

    services.AddMemoryCache();

    #region MQ Transmission (remote methods call)
    services.AddScoped<IRabbitClient, RabbitClient>();
    //
    services.AddScoped<IHelpdeskRemoteTransmissionService, TransmissionHelpdeskService>();
    //
    services.AddScoped<IWebRemoteTransmissionService, TransmissionWebService>();
    services.AddScoped<ITelegramRemoteTransmissionService, TransmissionTelegramService>();
    services.AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>();
    // 
    services.RegisterMqListener<RubricsListReceive, TProjectedRequestModel<int>?, RubricIssueHelpdeskLowModel[]?>();
    services.RegisterMqListener<RubricCreateOrUpdateReceive, RubricIssueHelpdeskModelDB?, int?>();
    services.RegisterMqListener<IssuesForUserSelectReceive, TPaginationRequestModel<GetIssuesForUserRequestModel>?, TPaginationResponseModel<IssueHelpdeskModel>?>();
    services.RegisterMqListener<IssueCreateOrUpdateReceive, TAuthRequestModel<IssueUpdateRequestModel>?, int?>();
    services.RegisterMqListener<MessageVoteReceive, TAuthRequestModel<VoteIssueRequestModel>?, bool?>();
    services.RegisterMqListener<MessageUpdateOrCreateReceive, TAuthRequestModel<IssueMessageHelpdeskBaseModel>?, int?>();
    services.RegisterMqListener<RubricMoveReceive, RowMoveModel?, bool?>();
    services.RegisterMqListener<IssueReadReceive, TAuthRequestModel<IssueReadRequestModel>?, IssueHelpdeskModelDB?>();
    services.RegisterMqListener<RubricReadReceive, int?, List<RubricIssueHelpdeskModelDB>?>();
    services.RegisterMqListener<SubscribeUpdateReceive, TAuthRequestModel<SubscribeUpdateRequestModel>?, bool?>();
    services.RegisterMqListener<SubscribesListReceive, TAuthRequestModel<int>?, SubscriberIssueHelpdeskModelDB[]?>();
    services.RegisterMqListener<ExecuterUpdateReceive, TAuthRequestModel<UserUpdateRequestModel>?, bool>();
    services.RegisterMqListener<MessagesListReceive, TAuthRequestModel<int>?, IssueMessageHelpdeskModelDB[]>();
    //
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

#if DEMO
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory = ss.ServiceProvider.GetRequiredService<IDbContextFactory<HelpdeskContext>>();
    HelpdeskContext context_seed = await helpdeskDbFactory.CreateDbContextAsync();
    if (!await context_seed.RubricsForIssues.AnyAsync())
    {
        List<RubricIssueHelpdeskModelDB> demo_rubrics = [
            new ()
            {
                Name = "Секретарь",
                NormalizedNameToUpper = "СЕКРЕТАРЬ",
                SortIndex = 1,
            },
            new ()
            {
                Name = "Техническая поддержка",
                NormalizedNameToUpper = "ТЕХНИЧЕСКАЯ ПОДДЕРЖКА",
                SortIndex = 2,
            },
            new ()
            {
                Name = "Другое",
                NormalizedNameToUpper = "ДРУГОЕ",
                SortIndex = 3,
            }];
        demo_rubrics[0].NestedRubrics = [new() { Name = "Справки", NormalizedNameToUpper = "СПРАВКИ", ParentRubric = demo_rubrics[0], SortIndex = 1 }, new() { Name = "Жалобы", NormalizedNameToUpper = "ЖАЛОБЫ", ParentRubric = demo_rubrics[0], SortIndex = 2 }];
        demo_rubrics[1].NestedRubrics = [new() { Name = "Линия 1", NormalizedNameToUpper = "ЛИНИЯ 1", ParentRubric = demo_rubrics[1], SortIndex = 1 }, new() { Name = "Линия 2", NormalizedNameToUpper = "ЛИНИЯ 2", ParentRubric = demo_rubrics[1], SortIndex = 2 }];
        await context_seed.AddRangeAsync(demo_rubrics);
        await context_seed.SaveChangesAsync();
    }
#endif

}

await app.RunAsync();