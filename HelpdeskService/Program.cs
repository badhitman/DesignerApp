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
using Microsoft.AspNetCore.Hosting;

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
string path_load;

builder.ConfigureHostConfiguration(configHost =>
{
    configHost.SetBasePath(curr_dir);
    path_load = Path.Combine(curr_dir, "appsettings.json");
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

        if (Directory.Exists(di.FullName))
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
    .Configure<HelpdeskConfigModel>(context.Configuration.GetSection("HelpdeskConfig"))
    ;

    services.AddScoped<IArticlesService, ArticlesService>();
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = context.Configuration.GetConnectionString($"RedisConnectionString{_modePrefix}");
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
    .AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>()
    .AddScoped<IIdentityRemoteTransmissionService, IdentityServiceTransmission>()
    ;
    // 
    services.HelpdeskRegisterMqListeners();
    //  
    #endregion
});

IHost app = builder.Build();

using (IServiceScope ss = app.Services.CreateScope())
{
    IOptions<HelpdeskConfigModel> wc_main = ss.ServiceProvider.GetRequiredService<IOptions<HelpdeskConfigModel>>();
    IWebRemoteTransmissionService webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebRemoteTransmissionService>();
    TelegramBotConfigModel wc_remote = await webRemoteCall.GetWebConfig();
    if (Uri.TryCreate(wc_remote.BaseUri, UriKind.Absolute, out _))
        wc_main.Value.Update(wc_remote.BaseUri);

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
                Name = "Секретарь",
                NormalizedNameUpper = "СЕКРЕТАРЬ",
                SortIndex = 1,
            },
            new ()
            {
                Name = "Техническая поддержка",
                NormalizedNameUpper = "ТЕХНИЧЕСКАЯ ПОДДЕРЖКА",
                SortIndex = 2,
            },
            new ()
            {
                Name = "Другое",
                NormalizedNameUpper = "ДРУГОЕ",
                SortIndex = 3,
            }];
        demo_rubrics[0].NestedRubrics = [new() { Name = "Справки", NormalizedNameUpper = "СПРАВКИ", Parent = demo_rubrics[0], SortIndex = 1 }, new() { Name = "Жалобы", NormalizedNameUpper = "ЖАЛОБЫ", Parent = demo_rubrics[0], SortIndex = 2 }];
        demo_rubrics[1].NestedRubrics = [new() { Name = "Линия 1", NormalizedNameUpper = "ЛИНИЯ 1", Parent = demo_rubrics[1], SortIndex = 1 }, new() { Name = "Линия 2", NormalizedNameUpper = "ЛИНИЯ 2", Parent = demo_rubrics[1], SortIndex = 2 }];
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
    //                Name = $"Тест {issues_demo_count++}",
    //                NormalizedNameUpper = $"ТЕСТ {issues_demo_count}",
    //                RubricIssueId = rubric_ids[random.Next(0, rubric_ids.Length)],
    //                Description = $"Доброго дня. Это demo описание {issues_demo_count}",
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