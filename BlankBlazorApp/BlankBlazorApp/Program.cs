using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlankBlazorApp.Components;
using System.Globalization;
using MudBlazor.Services;
using RemoteCallLib;
using IdentityLib;
using ServerLib;
using SharedLib;
using BlazorLib;
using NLog.Web;
using DbcLib;
using NLog;
using BlazorWebLib;
using BlankBlazorApp;
#if !DEBUG
using System.Reflection;
#endif

// Early init of NLog to allow startup and exception logging, before host is built
Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();
string _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? builder.Environment.EnvironmentName;
logger.Warn($"init main: {_environmentName}");

string _modePrefix = Environment.GetEnvironmentVariable(nameof(GlobalStaticConstants.TransmissionQueueNamePrefix)) ?? "";
if (!string.IsNullOrWhiteSpace(_modePrefix))
    GlobalStaticConstants.TransmissionQueueNamePrefix += _modePrefix.Trim();

string curr_dir = Directory.GetCurrentDirectory();
builder.Configuration.SetBasePath(curr_dir);
string path_load = Path.Combine(curr_dir, "appsettings.json");
if (Path.Exists(path_load))
{
    logger.Warn($"config load: {path_load}\n{File.ReadAllText(path_load)}");
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
}
else
    logger.Warn($"отсутствует: {path_load}");

path_load = Path.Combine(curr_dir, $"appsettings.{_environmentName}.json");
if (Path.Exists(path_load))
{
    logger.Warn($"config load: {path_load}\n{File.ReadAllText(path_load)}");
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
}
else
    logger.Warn($"отсутствует: {path_load}");

path_load = Path.Combine(curr_dir, $"bottom-menu.json");
if (Path.Exists(path_load))
{
    logger.Warn($"config load: {path_load}\n{File.ReadAllText(path_load)}");
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
}
else
    logger.Warn($"отсутствует: {path_load}");

path_load = Path.Combine(curr_dir, $"bottom-menu.{_environmentName}.json");
if (Path.Exists(path_load))
{
    logger.Warn($"config load: {path_load}\n{File.ReadAllText(path_load)}");
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
}
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

builder.Services.AddIdleCircuitHandler(options =>
    options.IdleTimeout = TimeSpan.FromMinutes(5));

builder.Services.AddOptions();
builder.Services
    .Configure<SmtpConfigModel>(builder.Configuration.GetSection("SmtpConfig"))
    .Configure<UserManageConfigModel>(builder.Configuration.GetSection("UserManage"))
    .Configure<ServerConfigModel>(builder.Configuration.GetSection("ServerConfig"))
    .Configure<RabbitMQConfigModel>(builder.Configuration.GetSection("RabbitMQConfig"))
    .Configure<TelegramBotConfigModel>(builder.Configuration.GetSection("WebConfig"))
    ;

NavMainMenuModel? mainNavMenu = builder.Configuration.GetSection("NavMenuConfig").Get<NavMainMenuModel>();
mainNavMenu ??= new NavMainMenuModel() { NavMenuItems = [new NavItemModel() { HrefNav = "", Title = "Home", IsNavLinkMatchAll = true }] };
builder.Services.AddCascadingValue(sp => mainNavMenu);

string connectionIdentityString = builder.Configuration.GetConnectionString($"IdentityConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'IdentityConnection{_modePrefix}' not found.");
builder.Services.AddDbContextFactory<IdentityAppDbContext>(opt =>
    opt.UseNpgsql(connectionIdentityString));

string connectionMainString = builder.Configuration.GetConnectionString($"MainConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'MainConnection{_modePrefix}' not found.");
builder.Services.AddDbContextFactory<MainDbAppContext>(opt =>
    opt.UseNpgsql(connectionMainString));

builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

builder.Services.AddMemoryCache();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers();
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<ApplicationRole>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddEntityFrameworkStores<IdentityAppDbContext>()
    .AddSignInManager()
    .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
    .AddDefaultTokenProviders()
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

builder.Services.AddHttpContextAccessor();

//Singleton
builder.Services
    .AddSingleton<IMailProviderService, MailProviderService>()
    .AddSingleton<IEmailSender<ApplicationUser>, IdentityEmailSender>()
    ;

// Scoped
builder.Services.AddScoped<IUsersAuthenticateService, UsersAuthenticateService>()
    .AddScoped<IUsersProfilesService, UsersProfilesService>()
    ;

#region MQ Transmission (remote methods call)
builder.Services.AddScoped<IRabbitClient, RabbitClient>();
//
builder.Services
    .AddScoped<ICommerceTransmission, CommerceTransmission>()
    .AddScoped<ITelegramTransmission, TelegramTransmission>()
    .AddScoped<IHelpdeskTransmission, HelpdeskTransmission>()
    .AddScoped<IStorageTransmission, StorageTransmission>()
    .AddScoped<IConstructorTransmission, ConstructorTransmission>()
    .AddScoped<IIdentityTransmission, IdentityTransmission>()
    .AddScoped<IWebTransmission, WebTransmission>();
//
builder.Services.WebAppRegisterMqListeners();
#endregion

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Append("Content-Security-Policy", "{POLICY STRING}");
//    await next();
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Map("/cloud-fs/read", ma => ma.UseMiddleware<ReadCloudFileMiddleware>());

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlankBlazorApp.Client._Imports).Assembly, typeof(BlazorWebLib._Imports).Assembly, typeof(BlazorLib._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

#if !DEBUG
Task? init_email_notify = null;
try
{
    init_email_notify = app.Services
        .GetRequiredService<IMailProviderService>()
        .SendTechnicalEmailNotificationAsync($"init main{(string.IsNullOrWhiteSpace(_modePrefix) ? "" : $" (prefix mode: {_modePrefix})")}: {Assembly.GetEntryAssembly()?.FullName}");

    if (init_email_notify is not null)
        await init_email_notify;
    else
        logger.Error($"init_email_notify is null. error {{807BC02B-E15E-4840-A564-FBC50CFBFFB8}}");
}
catch (Exception ex)
{
    logger.Error(ex);
}
#endif

app.Run();