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
        logger.Warn("Ñåêðåòû íå íàéäåíû");

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
    services.AddScoped<IWebRemoteTransmissionService, TransmissionWebService>()
    .AddScoped<ITelegramRemoteTransmissionService, TransmissionTelegramService>()
    .AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>();
    // 
    services.RegisterMqListener<RubricsListReceive, TProjectedRequestModel<int>?, RubricIssueHelpdeskLowModel[]?>()
    .RegisterMqListener<RubricCreateOrUpdateReceive, RubricIssueHelpdeskModelDB?, int?>()
    .RegisterMqListener<IssuesForUserSelectReceive, TPaginationRequestModel<GetIssuesForUserRequestModel>?, TPaginationResponseModel<IssueHelpdeskModel>?>()
    .RegisterMqListener<IssueCreateOrUpdateReceive, TAuthRequestModel<IssueUpdateRequestModel>?, int>()
    .RegisterMqListener<MessageVoteReceive, TAuthRequestModel<VoteIssueRequestModel>?, bool?>()
    .RegisterMqListener<MessageUpdateOrCreateReceive, TAuthRequestModel<IssueMessageHelpdeskBaseModel>?, int?>()
    .RegisterMqListener<RubricMoveReceive, RowMoveModel?, bool?>()
    .RegisterMqListener<IssueReadReceive, TAuthRequestModel<IssueReadRequestModel>?, IssueHelpdeskModelDB?>()
    .RegisterMqListener<RubricReadReceive, int?, List<RubricIssueHelpdeskModelDB>?>()
    .RegisterMqListener<SubscribeUpdateReceive, TAuthRequestModel<SubscribeUpdateRequestModel>?, bool?>()
    .RegisterMqListener<SubscribesListReceive, TAuthRequestModel<int>?, SubscriberIssueHelpdeskModelDB[]?>()
    .RegisterMqListener<ExecuterUpdateReceive, TAuthRequestModel<UserIssueModel>?, bool>()
    .RegisterMqListener<MessagesListReceive, TAuthRequestModel<int>?, IssueMessageHelpdeskModelDB[]>()
    .RegisterMqListener<StatusChangeReceive, TAuthRequestModel<StatusChangeRequestModel>?, bool>()
    .RegisterMqListener<PulseIssueReceive, TAuthRequestModel<PulseIssueBaseModel>?, bool>()
    .RegisterMqListener<PulseJournalReceive, TPaginationRequestModel<UserIssueModel>?, TPaginationResponseModel<PulseViewModel>>()
    .RegisterMqListener<TelegramMessageIncomingReceive, TelegramIncomingMessageModel?, bool>()
    .RegisterMqListener<ConsoleIssuesSelectReceive, TPaginationRequestModel<ConsoleIssuesRequestModel>?, TPaginationResponseModel<IssueHelpdeskModel>>();
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




#if DEBUG
#if DEMO
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory = ss.ServiceProvider.GetRequiredService<IDbContextFactory<HelpdeskContext>>();
    using HelpdeskContext context_seed = await helpdeskDbFactory.CreateDbContextAsync();
    List<RubricIssueHelpdeskModelDB> demo_rubrics = [.. await context_seed.RubricsForIssues.ToArrayAsync()];
    if (demo_rubrics.Count == 0)
    {
        demo_rubrics = [
            new ()
            {
                Name = "Ñåêðåòàðü",
                NormalizedNameUpper = "ÑÅÊÐÅÒÀÐÜ",
                SortIndex = 1,
            },
            new ()
            {
                Name = "Òåõíè÷åñêàÿ ïîääåðæêà",
                NormalizedNameUpper = "ÒÅÕÍÈ×ÅÑÊÀß ÏÎÄÄÅÐÆÊÀ",
                SortIndex = 2,
            },
            new ()
            {
                Name = "Äðóãîå",
                NormalizedNameUpper = "ÄÐÓÃÎÅ",
                SortIndex = 3,
            }];
        demo_rubrics[0].NestedRubrics = [new() { Name = "Ñïðàâêè", NormalizedNameUpper = "ÑÏÐÀÂÊÈ", ParentRubric = demo_rubrics[0], SortIndex = 1 }, new() { Name = "Æàëîáû", NormalizedNameUpper = "ÆÀËÎÁÛ", ParentRubric = demo_rubrics[0], SortIndex = 2 }];
        demo_rubrics[1].NestedRubrics = [new() { Name = "Ëèíèÿ 1", NormalizedNameUpper = "ËÈÍÈß 1", ParentRubric = demo_rubrics[1], SortIndex = 1 }, new() { Name = "Ëèíèÿ 2", NormalizedNameUpper = "ËÈÍÈß 2", ParentRubric = demo_rubrics[1], SortIndex = 2 }];
        await context_seed.AddRangeAsync(demo_rubrics);
        await context_seed.SaveChangesAsync();


    }

    if (!await context_seed.Issues.AnyAsync())
    {
        int[] rubric_ids = [.. demo_rubrics.Select(x => x.Id)];
        List<HelpdeskIssueStepsEnum> Steps = [.. Enum.GetValues(typeof(HelpdeskIssueStepsEnum)).Cast<HelpdeskIssueStepsEnum>()];

        List<IssueHelpdeskModelDB> issues = [];
        Random random = new();
        uint issues_demo_count = 1;
        foreach (HelpdeskIssueStepsEnum st in Steps)
        {
            int size = random.Next(3, 30);
            for (int i = 0; i < size; i++)
            {
                issues.Add(new()
                {
                    AuthorIdentityUserId = "",
                    Name = $"Òåñò {issues_demo_count++}",
                    NormalizedNameUpper = $"ÒÅÑÒ {issues_demo_count}",
                    RubricIssueId = rubric_ids[random.Next(0, rubric_ids.Length)],
                    Description = $"Äîáðîãî äíÿ. Ýòî demo îïèñàíèå {issues_demo_count}",
                    StepIssue = st,
                });
            }
        }
        await context_seed.AddRangeAsync(issues);
        await context_seed.SaveChangesAsync();
    }
#endif
#endif
}

await app.RunAsync();