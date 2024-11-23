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

Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder
    .Logging
    .ClearProviders()
    .AddNLog();

string _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? builder.Environment.EnvironmentName;
logger.Warn($"init main: {_environmentName}");

string _modePrefix = Environment.GetEnvironmentVariable(nameof(GlobalStaticConstants.TransmissionQueueNamePrefix)) ?? "";
if (!string.IsNullOrWhiteSpace(_modePrefix))
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
    logger.Warn($"отсутсвует: {path_load}");

path_load = Path.Combine(curr_dir, $"appsettings.{_environmentName}.json");
if (Path.Exists(path_load))
{
    logger.Warn($"config load: {path_load}\n{File.ReadAllText(path_load)}");
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
}
else
    logger.Warn($"отсутсвует: {path_load}");

// Secrets
void ReadSecrets(string dirName)
{
    string secretPath = Path.Combine("..", dirName);
    for (int i = 0; i < 5 && !Directory.Exists(secretPath); i++)
    {
        logger.Warn($"файл секретов не найден (продолжение следует...): {secretPath}");
        secretPath = Path.Combine("..", secretPath);
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
.Configure<MongoConfigModel>(builder.Configuration.GetSection("MongoDB"))
.Configure<RestApiConfigBaseModel>(builder.Configuration.GetSection("ApiAccess"))
;
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<UnhandledExceptionAttribute>();
builder.Services.AddAuthorization();

//MongoConfigModel _jo = builder.Configuration.GetSection("MongoDB").Get<MongoConfigModel>() ?? throw new Exception("Отсутствует конфигурация MonoDB");
//builder.Services.AddSingleton(new MongoClient(_jo.ToString()).GetDatabase(_jo.FilesSystemName));

RestApiConfigBaseModel restConf = builder.Configuration.GetSection("ApiAccess").Get<RestApiConfigBaseModel>() ?? throw new Exception("Отсутствует конфигурация ApiAccess");

// Add services to the container.
#region MQ Transmission (remote methods call)
builder.Services.AddScoped<IRabbitClient, RabbitClient>();
//
builder.Services
    .AddScoped<IWebRemoteTransmissionService, TransmissionWebService>()
    .AddScoped<ITelegramRemoteTransmissionService, TransmissionTelegramService>()
    .AddScoped<IHelpdeskRemoteTransmissionService, TransmissionHelpdeskService>()
    .AddScoped<ICommerceRemoteTransmissionService, TransmissionCommerceService>()
    .AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>();
#endregion

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