////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json.Converters;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System.Reflection;
using ApiRestService;
using RemoteCallLib;
using SharedLib;
using NLog.Web;
using NLog;
using OpenTelemetry;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Options;

Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder
    .Logging
    .ClearProviders()
    .AddNLog()
    .AddOpenTelemetry(logging =>
    {
        logging.IncludeFormattedMessage = true;
        logging.IncludeScopes = true;
    });

string _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? builder.Environment.EnvironmentName;
logger.Warn($"init main: {_environmentName}");

string _modePrefix = Environment.GetEnvironmentVariable(nameof(GlobalStaticConstants.TransmissionQueueNamePrefix)) ?? "";
if (!string.IsNullOrWhiteSpace(_modePrefix) && !GlobalStaticConstants.TransmissionQueueNamePrefix.EndsWith(_modePrefix))
    GlobalStaticConstants.TransmissionQueueNamePrefix += _modePrefix.Trim();

logger.Warn($"Префикс рабочего контура/контекста: {(string.IsNullOrWhiteSpace(_modePrefix) ? "НЕ ИСПОЛЬЗУЕТСЯ" : $"`{_modePrefix}`")}");

string curr_dir = Directory.GetCurrentDirectory();
builder.Configuration.SetBasePath(curr_dir);

builder.Configuration.SetBasePath(curr_dir);
string path_load = Path.Combine(curr_dir, "appsettings.json");
if (Path.Exists(path_load))
{
    logger.Warn($"config load: {path_load}\n{File.ReadAllText(path_load)}");
    builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
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
.Configure<RestApiConfigBaseModel>(builder.Configuration.GetSection("ApiAccess"))
.Configure<PartUploadSessionConfigModel>(builder.Configuration.GetSection("PartUploadSessionConfig"))
;

builder.Services.AddOptions();
builder.Services.AddMemoryCache();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString($"RedisConnectionString{_modePrefix}");
})
.AddSingleton<IManualCustomCacheService, ManualCustomCacheService>();

builder.Services.AddTransient<UnhandledExceptionAttribute>();
builder.Services.AddAuthorization();

RestApiConfigBaseModel restConf = builder.Configuration.GetSection("ApiAccess").Get<RestApiConfigBaseModel>() ?? throw new Exception("Отсутствует конфигурация ApiAccess");

// Add services to the container.
#region MQ Transmission (remote methods call)
string appName = typeof(Program).Assembly.GetName().Name ?? "AssemblyName";
builder.Services.AddSingleton<IRabbitClient>(x =>
    new RabbitClient(x.GetRequiredService<IOptions<RabbitMQConfigModel>>(),
                x.GetRequiredService<ILogger<RabbitClient>>(),
                appName));
//
builder.Services
    .AddScoped<IWebTransmission, WebTransmission>()
    .AddScoped<ITelegramTransmission, TelegramTransmission>()
    .AddScoped<IHelpdeskTransmission, HelpdeskTransmission>()
    .AddScoped<ICommerceTransmission, CommerceTransmission>()
    .AddScoped<IStorageTransmission, StorageTransmission>();
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
    metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
});

// Add Tracing for ASP.NET Core and our custom ActivitySource and export via OTLP
otel.WithTracing(tracing =>
{
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    tracing.AddSource($"OTel.{appName}");
});
builder.Services.AddScoped<IToolsSystemService, ToolsSystemService>();

builder.Services
    .AddControllers(options => options.Filters.Add(typeof(LoggerActionFilter)))
    .AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; options.SerializerSettings.Converters.Add(new StringEnumConverter() { }); });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options =>
        {
            options.LoginPath = new PathString("/auth/login");
            options.AccessDeniedPath = new PathString("/auth/denied");
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
          {
              options.SwaggerDoc("v1", new OpenApiInfo
              {
                  Version = "v1",
                  Title = "tools - REST API",
                  Description = $"Удалённый RESTful доступ к <a href='https://github.com/badhitman/DesignerApp'>https://github.com/badhitman/DesignerApp</a>. Токен доступа следует передавать с каждым запросом через заголовок с именем '{restConf.TokenAccessHeaderName}'",
                  Contact = new OpenApiContact
                  {
                      Email = "ru.usa@mail.ru",
                      Url = new("https://github.com/badhitman"),
                      Name = "badhitman"
                  }
              });

              options.UseInlineDefinitionsForEnums();
              options.IncludeXmlComments(GetXmlCommentsPath(), includeControllerXmlComments: true);

              string commentsFileName = nameof(SharedLib) + ".xml";
              string baseDirectory = AppContext.BaseDirectory;
              options.IncludeXmlComments(Path.Combine(baseDirectory, commentsFileName), includeControllerXmlComments: true);
          });
builder.Services.AddSwaggerGenNewtonsoftSupport();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("v1/swagger.json", "tools - Rest/API");
    options.InjectStylesheet("/assets/css/swagger-style.css");
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseForwardedHeaders();
app.MapControllers();
#if DEBUG
app.UseDeveloperExceptionPage();
#else
app.UseExceptionHandler("/Error");
#endif

app.UseAuthentication();
app.UseWhen(context =>
{
    return context.Request.Path.Value?.StartsWith("/api") ?? false;
}, appBuilder =>
{
    app.UseMiddleware<PassageMiddleware>();
});

app.UseAuthorization();
app.Run();

static string GetXmlCommentsPath()
{
    string baseDirectory = AppContext.BaseDirectory;
    string commentsFileName = Assembly.GetEntryAssembly()!.GetName().Name + ".xml";
    return Path.Combine(baseDirectory, commentsFileName);
}