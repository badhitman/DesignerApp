using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlankBlazorApp.Components;
using Transmission.Receives.web;
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

// Early init of NLog to allow startup and exception logging, before host is built
Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();
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

IWebHostEnvironment env = builder.Environment;
logger.Warn($"init main: {env.EnvironmentName}");
string curr_dir = Directory.GetCurrentDirectory();
builder.Configuration.SetBasePath(curr_dir);
string path_load = Path.Combine(curr_dir, "appsettings.json");
if (Path.Exists(path_load))
{
    logger.Warn($"config load: {path_load}\n{File.ReadAllText(path_load)}");
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
}

path_load = Path.Combine(curr_dir, $"appsettings.{env.EnvironmentName}.json");
if (Path.Exists(path_load))
{
    logger.Warn($"config load: {path_load}\n{File.ReadAllText(path_load)}");
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
}

path_load = Path.Combine(curr_dir, $"bottom-menu.json");
if (Path.Exists(path_load))
{
    logger.Warn($"config load: {path_load}\n{File.ReadAllText(path_load)}");
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
}

path_load = Path.Combine(curr_dir, $"bottom-menu.{env.EnvironmentName}.json");
if (Path.Exists(path_load))
{
    logger.Warn($"config load: {path_load}\n{File.ReadAllText(path_load)}");
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
}

// Secrets
string secretPath = Path.Combine("..", "secrets");
for (int i = 0; i < 5 && !Directory.Exists(secretPath); i++)
    secretPath = Path.Combine("..", secretPath);

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
    logger.Warn("Секреты не найдены");

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

builder.Services.AddOptions();
builder.Services
    .Configure<SmtpConfigModel>(builder.Configuration.GetSection("SmtpConfig"))
    .Configure<UserManageConfigModel>(builder.Configuration.GetSection("UserManage"))
    .Configure<ServerConfigModel>(builder.Configuration.GetSection("ServerConfig"))
    .Configure<RabbitMQConfigModel>(builder.Configuration.GetSection("RabbitMQConfig"))
    .Configure<WebConfigModel>(builder.Configuration.GetSection("WebConfig"))
    ;

NavMainMenuModel? mainNavMenu = builder.Configuration.GetSection("NavMenuConfig").Get<NavMainMenuModel>();
mainNavMenu ??= new NavMainMenuModel() { NavMenuItems = [new NavItemModel() { HrefNav = "", Title = "Home", IsNavLinkMatchAll = true }] };
builder.Services.AddCascadingValue(sp => mainNavMenu);

string connectionIdentityString = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");
builder.Services.AddDbContextFactory<IdentityAppDbContext>(opt =>
    opt.UseNpgsql(connectionIdentityString));

string connectionMainString = builder.Configuration.GetConnectionString("MainConnection") ?? throw new InvalidOperationException("Connection string 'MainConnection' not found.");
builder.Services.AddDbContextFactory<MainDbAppContext>(opt =>
    opt.UseNpgsql(connectionMainString));
builder.Services.AddMemoryCache();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers();
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<ApplicationRole>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddEntityFrameworkStores<IdentityAppDbContext>()
    .AddSignInManager()
    .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
    .AddDefaultTokenProviders();

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
    .AddSingleton<IEmailSender<ApplicationUser>, IdentityEmailSender>();

// Scoped
builder.Services.AddScoped<IUsersAuthenticateService, UsersAuthenticateService>()
    .AddScoped<IUsersProfilesService, UsersProfilesService>()
    .AddScoped<IWebAppService, WebAppService>();
//
builder.Services.AddScoped<IdentityTools>();

#region MQ Transmission (remote methods call)
builder.Services.AddScoped<IRabbitClient, RabbitClient>();
//
builder.Services
    .AddScoped<ICommerceRemoteTransmissionService, TransmissionCommerceService>()
    .AddScoped<ITelegramRemoteTransmissionService, TransmissionTelegramService>()
    .AddScoped<IHelpdeskRemoteTransmissionService, TransmissionHelpdeskService>()
    .AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>()
    .AddScoped<IConstructorRemoteTransmissionService, TransmissionConstructorService>()
    .AddScoped<IWebRemoteTransmissionService, TransmissionWebService>();
//
builder.Services.RegisterMqListener<UpdateTelegramUserReceive, CheckTelegramUserHandleModel, CheckTelegramUserAuthModel?>()
    .RegisterMqListener<TelegramJoinAccountConfirmReceive, TelegramJoinAccountConfirmModel, object?>()
    .RegisterMqListener<TelegramJoinAccountDeleteReceive, long, object?>()
    .RegisterMqListener<GetWebConfigReceive, object?, WebConfigModel>()
    .RegisterMqListener<UpdateTelegramMainUserMessageReceive, MainUserMessageModel, object?>()
    .RegisterMqListener<GetTelegramUserReceive, long, TelegramUserBaseModel>()
    .RegisterMqListener<GetUsersOfIdentityReceive, string[], UserInfoModel[]>()
    .RegisterMqListener<SendEmailReceive, SendEmailRequestModel, bool>()
    .RegisterMqListener<GetUserIdentityByTelegramReceive, long[], UserInfoModel[]>()
    .RegisterMqListener<SetRoleForUserReceive, SetRoleFoeUserRequestModel, string[]>()
    .RegisterMqListener<SelectUsersOfIdentityReceive, TPaginationRequestModel<SimpleBaseRequestModel>, TPaginationResponseModel<UserInfoModel?>>();
#endregion

WebApplication app = builder.Build();

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

#if DEBUG

#else
Task? init_email_notify = null;

try
{
    init_email_notify = app.Services
        .GetRequiredService<IMailProviderService>()
        .SendTechnicalEmailNotificationAsync($"init main: {Assembly.GetEntryAssembly()?.FullName}");

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