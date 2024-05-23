using BlankBlazorApp.Components;
using BlazorLib;
using DbcLib;
using IdentityLib;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using ServerLib;
using SharedLib;
using System.Globalization;
using System.Reflection;
using Transmission.Receives.web;

// Early init of NLog to allow startup and exception logging, before host is built
Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

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

string curr_dir = Directory.GetCurrentDirectory();
builder.Configuration.SetBasePath(curr_dir);
if (Path.Exists(Path.Combine(curr_dir, "appsettings.json")))
    builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

if (Path.Exists(Path.Combine(curr_dir, $"appsettings.{env.EnvironmentName}.json")))
    builder.Configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Secrets
string secretPath = Path.Combine("..", "secrets");
for (int i = 0; i < 5 && !Directory.Exists(secretPath); i++)
    secretPath = Path.Combine("..", secretPath);
if (Directory.Exists(secretPath))
    foreach (string secret in Directory.GetFiles(secretPath, $"*.json"))
        builder.Configuration.AddJsonFile(Path.GetFullPath(secret), optional: true, reloadOnChange: true);
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

string connectionIdentityString = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");
builder.Services.AddDbContextFactory<IdentityAppDbContext>(opt =>
    opt.UseSqlite(connectionIdentityString));

string connectionMainString = builder.Configuration.GetConnectionString("MainConnection") ?? throw new InvalidOperationException("Connection string 'MainConnection' not found.");
builder.Services.AddDbContextFactory<MainDbAppContext>(opt =>
    opt.UseSqlite(connectionMainString));
builder.Services.AddDbContext<MainDbAppContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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
builder.Services.AddSingleton<IMailProviderService, MailProviderService>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityEmailSender>();

// Scoped
builder.Services.AddScoped<IUsersAuthenticateService, UsersAuthenticateService>();
builder.Services.AddScoped<IUsersProfilesService, UsersProfilesService>();
builder.Services.AddScoped<ITelegramWebService, TelegramWebService>();

#region MQ Transmission (remote methods call)
builder.Services.AddScoped<IRabbitClient, RabbitClient>();
builder.Services.AddScoped<ITelegramRemoteTransmissionService, TransmissionTelegramService>();
//
builder.Services.RegisterMqttListener<UpdateTelegramUserReceive, CheckTelegramUserHandleModel, CheckTelegramUserModel?>();
builder.Services.RegisterMqttListener<TelegramJoinAccountConfirmReceive, TelegramJoinAccountConfirmModel, object?>();
builder.Services.RegisterMqttListener<TelegramJoinAccountDeleteReceive, long, object?>();
builder.Services.RegisterMqttListener<GetWebConfigReceive, object?, WebConfigModel>();
builder.Services.RegisterMqttListener<UpdateTelegramMainUserMessageReceive, MainUserMessageModel, object?>();
builder.Services.RegisterMqttListener<GetTelegramUserReceive, long, TelegramUserBaseModelDb>();
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlankBlazorApp.Client._Imports).Assembly, typeof(BlazorWebLib._Imports).Assembly, typeof(BlazorLib._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

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

app.Run();