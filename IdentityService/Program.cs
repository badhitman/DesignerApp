////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using RemoteCallLib;
using IdentityLib;
using ServerLib;
using SharedLib;
using NLog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace IdentityService;

public class Program
{
    public static void Main(string[] args)
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        // NLog: Setup NLog for Dependency injection
        builder.Logging.ClearProviders();
        builder.Logging.AddNLog();

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
            .Configure<RabbitMQConfigModel>(builder.Configuration.GetSection("RabbitMQConfig"))
            .Configure<SmtpConfigModel>(builder.Configuration.GetSection("SmtpConfig"))
            ;

        string connectionIdentityString = builder.Configuration.GetConnectionString($"IdentityConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'IdentityConnection{_modePrefix}' not found.");
        builder.Services.AddDbContextFactory<IdentityAppDbContext>(opt =>
            opt.UseNpgsql(connectionIdentityString));

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<ApplicationRole>()
            .AddRoleManager<RoleManager<ApplicationRole>>()
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            ;

        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        builder.Services.AddLocalization(lo => lo.ResourcesPath = "Resources");
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            CultureInfo[] supportedCultures = [new CultureInfo("en-US"), new CultureInfo("ru-RU")];
            options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            IRequestCultureProvider? defaultCookieRequestProvider =
                options.RequestCultureProviders.FirstOrDefault(rcp =>
                    rcp.GetType() == typeof(CookieRequestCultureProvider));
            if (defaultCookieRequestProvider != null)
                options.RequestCultureProviders.Remove(defaultCookieRequestProvider);

            options.RequestCultureProviders.Insert(0,
                new CookieRequestCultureProvider()
                {
                    CookieName = ".AspNetCore.Culture",
                    Options = options
                });
        });

        //Singleton
        builder.Services
            .AddSingleton<IMailProviderService, MailProviderService>()
            .AddSingleton<IEmailSender<ApplicationUser>, IdentityEmailSender>()
            ;

        // Scoped
        builder.Services.AddScoped<IIdentityTools, IdentityTools>();

         //builder.Services
         //   .AddScoped<UserManager<ApplicationUser>>()
         //   //.AddScoped<RoleManager<ApplicationUser>>()
         //   ;

        #region MQ Transmission (remote methods call)
        builder.Services.AddScoped<IRabbitClient, RabbitClient>();

        builder.Services
            .AddScoped<ITelegramTransmission, TelegramTransmission>()
            .AddScoped<IHelpdeskTransmission, HelpdeskTransmission>()
            .AddScoped<IWebTransmission, WebTransmission>()
            .AddScoped<IStorageTransmission, StorageTransmission>()
            ;

        builder.Services.IdentityRegisterMqListeners();
        #endregion

        builder.Services.AddMemoryCache();
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString($"RedisConnectionString{_modePrefix}");
        });
        builder.Services.AddSingleton<IManualCustomCacheService, ManualCustomCacheService>();

        builder.Services.AddOptions();

        IHost host = builder.Build();
        host.Run();
    }
}