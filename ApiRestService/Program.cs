////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json.Converters;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System.Reflection;
using ApiRestService;
using RemoteCallLib;
using SharedLib;
using NLog.Web;
using NLog;
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;

Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Warn("init main");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder
    .Logging
    .ClearProviders()
    .AddNLog();

string curr_dir = Directory.GetCurrentDirectory();
builder.Configuration.SetBasePath(curr_dir);

builder.Configuration.SetBasePath(curr_dir);
if (Path.Exists(Path.Combine(curr_dir, "appsettings.json")))
    builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

#if DEBUG
if (Path.Exists(Path.Combine(curr_dir, "appsettings.Development.json")))
    builder.Configuration.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
#else
    if (Path.Exists(Path.Combine(curr_dir, "appsettings.Production.json")))
        builder.Configuration.AddJsonFile($"appsettings.Production.json", optional: true, reloadOnChange: true);
#endif

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

builder.Services
.Configure<RabbitMQConfigModel>(builder.Configuration.GetSection("RabbitMQConfig"))
.Configure<MongoConfigModel>(builder.Configuration.GetSection("MongoDB"))
;
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.AddAuthorization();

MongoConfigModel _jo = builder.Configuration.GetSection("MongoDB").Get<MongoConfigModel>()!;
builder.Services.AddSingleton(new MongoClient(_jo.ToString()).GetDatabase(_jo.FilesSystemName));

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
                  Description = "Удалённый RESTful доступ к <a href='https://github.com/badhitman/DesignerApp'>https://github.com/badhitman/DesignerApp</a>",
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