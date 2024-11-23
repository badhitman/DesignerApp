using Transmission.Receives.helpdesk;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using RemoteCallLib;
using SharedLib;
using NLog.Web;
using DbcLib;
using NLog;
using HelpdeskService;
using Microsoft.Extensions.Options;

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


ConfigurationBuilder bc = new();
bc.AddCommandLine(args);
IConfigurationRoot cb = bc.Build();
string _modePrefix = cb[nameof(GlobalStaticConstants.TransmissionQueueNamePrefix)] ?? "";
if (!string.IsNullOrWhiteSpace(_modePrefix))
    GlobalStaticConstants.TransmissionQueueNamePrefix += _modePrefix.Trim();

builder.ConfigureHostConfiguration(configHost =>
{
    string curr_dir = Directory.GetCurrentDirectory();
    configHost.SetBasePath(curr_dir);
    string path_load = Path.Combine(curr_dir, "appsettings.json");
    if (Path.Exists(path_load))
        configHost.AddJsonFile(path_load, optional: true, reloadOnChange: true);

#if DEBUG
    path_load = Path.Combine(curr_dir, "appsettings.Development.json");
    if (Path.Exists(path_load))
        configHost.AddJsonFile(path_load, optional: true, reloadOnChange: true);
    else
        logger.Warn($"����������: {path_load}");
#else
    path_load = Path.Combine(curr_dir, "appsettings.Production.json");
    if (Path.Exists(path_load))
        configHost.AddJsonFile(path_load, optional: true, reloadOnChange: true);
    else
        logger.Warn($"����������: {path_load}");
#endif

    // Secrets
    void ReadSecrets(string dirName)
    {
        string secretPath = Path.Combine("..", dirName);
        for (int i = 0; i < 5 && !Directory.Exists(secretPath); i++)
        {
            logger.Warn($"���� �������� �� ������ (����������� �������...): {secretPath}");
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
            logger.Warn($"������� `{dirName}` �� ������� (������)");
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
    .Configure<HelpdeskConfigModel>(context.Configuration.GetSection("HelpdeskConfig"))
    ;

    services.AddScoped<IArticlesService, ArticlesService>();
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = context.Configuration.GetConnectionString("RedisConnectionString");
        // options.InstanceName = "app.";
    });

    services.AddOptions();
    services.AddSingleton<IManualCustomCacheService, ManualCustomCacheService>();
    string connectionIdentityString = context.Configuration.GetConnectionString($"HelpdeskConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'HelpdeskConnection{_modePrefix}' not found.");
    services.AddDbContextFactory<HelpdeskContext>(opt =>
    {
        opt.UseNpgsql(connectionIdentityString);

#if DEBUG
        opt.EnableSensitiveDataLogging(true);
#endif
    });

    services.AddMemoryCache();

    #region MQ Transmission (remote methods call)
    services.AddScoped<IRabbitClient, RabbitClient>();
    //
    services.AddScoped<IHelpdeskRemoteTransmissionService, TransmissionHelpdeskService>()
    .AddScoped<IWebRemoteTransmissionService, TransmissionWebService>()
    .AddScoped<ITelegramRemoteTransmissionService, TransmissionTelegramService>()
    .AddScoped<ICommerceRemoteTransmissionService, TransmissionCommerceService>()
    .AddScoped<IHelpdeskService, HelpdeskImplementService>()
    .AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>();
    // 
    services.RegisterMqListener<RubricsListReceive, RubricsListRequestModel?, RubricBaseModel[]?>()
    .RegisterMqListener<RubricCreateOrUpdateReceive, RubricIssueHelpdeskModelDB?, int?>()
    .RegisterMqListener<IssuesSelectReceive, TPaginationRequestModel<SelectIssuesRequestModel>?, TPaginationResponseModel<IssueHelpdeskModel>?>()
    .RegisterMqListener<ArticlesSelectReceive, TPaginationRequestModel<SelectArticlesRequestModel>?, TPaginationResponseModel<ArticleModelDB>?>()
    .RegisterMqListener<IssueCreateOrUpdateReceive, TAuthRequestModel<IssueUpdateRequestModel>?, int>()
    .RegisterMqListener<MessageVoteReceive, TAuthRequestModel<VoteIssueRequestModel>?, bool?>()
    .RegisterMqListener<MessageUpdateOrCreateReceive, TAuthRequestModel<IssueMessageHelpdeskBaseModel>?, int?>()
    .RegisterMqListener<RubricMoveReceive, RowMoveModel?, bool?>()
    .RegisterMqListener<SetWebConfigReceive, HelpdeskConfigModel?, object?>()
    .RegisterMqListener<UpdateRubricsForArticleReceive, ArticleRubricsSetModel?, bool?>()
    .RegisterMqListener<ArticlesReadReceive, int[]?, ArticleModelDB[]?>()
    .RegisterMqListener<ArticleCreateOrUpdateReceive, ArticleModelDB?, int?>()
    .RegisterMqListener<IssuesReadReceive, TAuthRequestModel<IssuesReadRequestModel>?, IssueHelpdeskModelDB[]?>()
    .RegisterMqListener<RubricReadReceive, int?, List<RubricIssueHelpdeskModelDB>?>()
    .RegisterMqListener<RubricsGetReceive, int[]?, List<RubricIssueHelpdeskModelDB>?>()
    .RegisterMqListener<SubscribeUpdateReceive, TAuthRequestModel<SubscribeUpdateRequestModel>?, bool?>()
    .RegisterMqListener<SubscribesListReceive, TAuthRequestModel<int>?, SubscriberIssueHelpdeskModelDB[]?>()
    .RegisterMqListener<ExecuterUpdateReceive, TAuthRequestModel<UserIssueModel>?, bool>()
    .RegisterMqListener<MessagesListReceive, TAuthRequestModel<int>?, IssueMessageHelpdeskModelDB[]>()
    .RegisterMqListener<StatusChangeReceive, TAuthRequestModel<StatusChangeRequestModel>?, bool>()
    .RegisterMqListener<PulseIssueReceive, PulseRequestModel?, bool>()
    .RegisterMqListener<PulseJournalReceive, TPaginationRequestModel<UserIssueModel>?, TPaginationResponseModel<PulseViewModel>>()
    .RegisterMqListener<TelegramMessageIncomingReceive, TelegramIncomingMessageModel?, bool>()
    .RegisterMqListener<ConsoleIssuesSelectReceive, TPaginationRequestModel<ConsoleIssuesRequestModel>?, TPaginationResponseModel<IssueHelpdeskModel>>();
    //  
    #endregion
});

IHost app = builder.Build();

using (IServiceScope ss = app.Services.CreateScope())
{
    IOptions<HelpdeskConfigModel> wc_main = ss.ServiceProvider.GetRequiredService<IOptions<HelpdeskConfigModel>>();
    IWebRemoteTransmissionService webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebRemoteTransmissionService>();
    TResponseModel<TelegramBotConfigModel?> wc_remote = await webRemoteCall.GetWebConfig();
    if (wc_remote.Response is not null && wc_remote.Success() && Uri.TryCreate(wc_remote.Response.BaseUri, UriKind.Absolute, out _))
        wc_main.Value.Update(wc_remote.Response.BaseUri);

#if DEBUG
#if DEMO
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory = ss.ServiceProvider.GetRequiredService<IDbContextFactory<HelpdeskContext>>();
    using HelpdeskContext context_seed = await helpdeskDbFactory.CreateDbContextAsync();
    List<RubricIssueHelpdeskModelDB> demo_rubrics = [.. await context_seed.Rubrics.ToArrayAsync()];
    if (demo_rubrics.Count == 0)
    {
        demo_rubrics = [
            new ()
            {
                Name = "���������",
                NormalizedNameUpper = "���������",
                SortIndex = 1,
            },
            new ()
            {
                Name = "����������� ���������",
                NormalizedNameUpper = "����������� ���������",
                SortIndex = 2,
            },
            new ()
            {
                Name = "������",
                NormalizedNameUpper = "������",
                SortIndex = 3,
            }];
        demo_rubrics[0].NestedRubrics = [new() { Name = "�������", NormalizedNameUpper = "�������", ParentRubric = demo_rubrics[0], SortIndex = 1 }, new() { Name = "������", NormalizedNameUpper = "������", ParentRubric = demo_rubrics[0], SortIndex = 2 }];
        demo_rubrics[1].NestedRubrics = [new() { Name = "����� 1", NormalizedNameUpper = "����� 1", ParentRubric = demo_rubrics[1], SortIndex = 1 }, new() { Name = "����� 2", NormalizedNameUpper = "����� 2", ParentRubric = demo_rubrics[1], SortIndex = 2 }];
        await context_seed.AddRangeAsync(demo_rubrics);
        await context_seed.SaveChangesAsync();
    }

    //if (!await context_seed.Issues.AnyAsync())
    //{
    //    int[] rubric_ids = [.. demo_rubrics.Select(x => x.Id)];
    //    List<HelpdeskIssueStepsEnum> Steps = [.. Enum.GetValues(typeof(HelpdeskIssueStepsEnum)).Cast<HelpdeskIssueStepsEnum>()];

    //    List<IssueHelpdeskModelDB> issues = [];
    //    Random random = new();
    //    uint issues_demo_count = 1;
    //    foreach (HelpdeskIssueStepsEnum st in Steps)
    //    {
    //        int size = random.Next(3, 30);
    //        for (int i = 0; i < size; i++)
    //        {
    //            issues.Add(new()
    //            {
    //                AuthorIdentityUserId = "",
    //                Name = $"���� {issues_demo_count++}",
    //                NormalizedNameUpper = $"���� {issues_demo_count}",
    //                RubricIssueId = rubric_ids[random.Next(0, rubric_ids.Length)],
    //                Description = $"������� ���. ��� demo �������� {issues_demo_count}",
    //                StepIssue = st,
    //            });
    //        }
    //    }
    //    await context_seed.AddRangeAsync(issues);
    //    await context_seed.SaveChangesAsync();
    //}
#endif
#endif
}

await app.RunAsync();