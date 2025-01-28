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
using OpenTelemetry;
using System.Diagnostics.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace HelpdeskService;

/// <summary>
/// Program
/// </summary>
public class Program
{
    /// <summary>
    /// Main
    /// </summary>
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        Logger logger = LogManager.GetCurrentClassLogger();
        builder.AddServiceDefaults();

        // NLog: Setup NLog for Dependency injection
        builder.Logging
            .ClearProviders()
            .AddNLog()
            .AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true; logging.IncludeScopes = true;
            });

        string _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Other";
        logger.Warn($"init main: {_environmentName}");
        string _modePrefix = Environment.GetEnvironmentVariable(nameof(GlobalStaticConstants.TransmissionQueueNamePrefix)) ?? "";
        if (!string.IsNullOrWhiteSpace(_modePrefix))
            GlobalStaticConstants.TransmissionQueueNamePrefix += _modePrefix.Trim();
        string curr_dir = Directory.GetCurrentDirectory();


        builder.Configuration.SetBasePath(curr_dir);
        string path_load = Path.Combine(curr_dir, "appsettings.json");
        if (Path.Exists(path_load))
            builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);

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


        builder.Services
        .Configure<RabbitMQConfigModel>(builder.Configuration.GetSection(RabbitMQConfigModel.Configuration))
        .Configure<HelpdeskConfigModel>(builder.Configuration.GetSection(HelpdeskConfigModel.Configuration))
        ;

        builder.Services.AddScoped<IArticlesService, ArticlesService>();
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString($"RedisConnectionString{_modePrefix}");
        });

        builder.Services.AddOptions();
        builder.Services.AddSingleton<IManualCustomCacheService, ManualCustomCacheService>();
        string connectionIdentityString = builder.Configuration.GetConnectionString($"HelpdeskConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'HelpdeskConnection{_modePrefix}' not found.");
        builder.Services.AddDbContextFactory<HelpdeskContext>(opt =>
    {
        opt.UseNpgsql(connectionIdentityString);

#if DEBUG
        opt.EnableSensitiveDataLogging(true);
#endif
    });

        builder.Services.AddMemoryCache();

        #region MQ Transmission (remote methods call)
        string appName = typeof(Program).Assembly.GetName().Name ?? "AssemblyName";
        builder.Services.AddSingleton<IRabbitClient>(x =>
        new RabbitClient(x.GetRequiredService<IOptions<RabbitMQConfigModel>>(),
                    x.GetRequiredService<ILogger<RabbitClient>>(),
                    appName));
        //
        builder.Services.AddScoped<IHelpdeskTransmission, HelpdeskTransmission>()
    .AddScoped<IWebTransmission, WebTransmission>()
    .AddScoped<ITelegramTransmission, TelegramTransmission>()
    .AddScoped<ICommerceTransmission, CommerceTransmission>()
    .AddScoped<IHelpdeskService, HelpdeskImplementService>()
    .AddScoped<IStorageTransmission, StorageTransmission>()
    .AddScoped<IIdentityTransmission, IdentityTransmission>()
    ;
        // 
        builder.Services.HelpdeskRegisterMqListeners();
        //  
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
            tracing.AddSource($"OTel.{appName}");
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
        });

        IHost app = builder.Build();

        using (IServiceScope ss = app.Services.CreateScope())
        {
            IOptions<HelpdeskConfigModel> wc_main = ss.ServiceProvider.GetRequiredService<IOptions<HelpdeskConfigModel>>();
            IWebTransmission webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebTransmission>();
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
    }
}