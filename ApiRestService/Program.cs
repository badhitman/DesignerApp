////////////////////////////////////////////////
// � https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using ApiRestService;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using RemoteCallLib;
using SharedLib;
using System.Reflection;

Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

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
    logger.Warn("������� �� �������");

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

builder.Services
.Configure<RabbitMQConfigModel>(builder.Configuration.GetSection("RabbitMQConfig"))
;
builder.Services.AddOptions();
builder.Services.AddMemoryCache();

// Add services to the container.
#region MQ Transmission (remote methods call)
builder.Services.AddScoped<IRabbitClient, RabbitClient>();
//
//    builder.Services;
//
builder.Services.AddScoped<IWebRemoteTransmissionService, TransmissionWebService>()
.AddScoped<ITelegramRemoteTransmissionService, TransmissionTelegramService>()
.AddScoped<IHelpdeskRemoteTransmissionService, TransmissionHelpdeskService>()
.AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>();
// 
//    builder.Services.RegisterMqListener<RubricsListReceive, TProjectedRequestModel<int>?, RubricIssueHelpdeskLowModel[]?>()
//  
#endregion

builder.Services
    .AddControllers(options => options.Filters.Add(typeof(LoggerActionFilter)))
    .AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; options.SerializerSettings.Converters.Add(new StringEnumConverter() { }); });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
          {
              options.SwaggerDoc("v1", new OpenApiInfo
              {
                  Version = "v1",
                  Title = "tools - Rest/API",
                  Contact = new OpenApiContact { Email = "ru.usa@mail.ru" }
              });

              options.UseInlineDefinitionsForEnums();
              options.IncludeXmlComments(GetXmlCommentsPath(), includeControllerXmlComments: true);

              string commentsFileName = nameof(SharedLib) + ".xml";
              string baseDirectory = AppContext.BaseDirectory;
              options.IncludeXmlComments(Path.Combine(baseDirectory, commentsFileName), includeControllerXmlComments: true);
          });
builder.Services.AddSwaggerGenNewtonsoftSupport();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

static string GetXmlCommentsPath()
{
    string baseDirectory = AppContext.BaseDirectory;
    string commentsFileName = Assembly.GetEntryAssembly()!.GetName().Name + ".xml";
    return Path.Combine(baseDirectory, commentsFileName);
}