using DbcLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using RemoteCallLib;
using SharedLib;
using StorageService;
using Transmission.Receives.storage;

// Early init of NLog to allow startup and exception logging, before host is built
Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Warn("init main");
IHostBuilder builder = Host.CreateDefaultBuilder(args);

// NLog: Setup NLog for Dependency injection
builder.ConfigureLogging((lc, lb) =>
{
    lb.ClearProviders();
    lb.AddNLog();
});
builder.UseNLog();

builder.ConfigureHostConfiguration(configHost =>
{
    string curr_dir = Directory.GetCurrentDirectory();
    configHost.SetBasePath(curr_dir);
    if (Path.Exists(Path.Combine(curr_dir, "appsettings.json")))
        configHost.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

#if DEBUG
    if (Path.Exists(Path.Combine(curr_dir, "appsettings.Development.json")))
        configHost.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
#else
    if (Path.Exists(Path.Combine(curr_dir, "appsettings.Production.json")))
        configHost.AddJsonFile($"appsettings.Production.json", optional: true, reloadOnChange: true);
#endif

    // Secrets
    string secretPath = Path.Combine("..", "secrets");
    for (int i = 0; i < 5 && !Directory.Exists(secretPath); i++)
        secretPath = Path.Combine("..", secretPath);
    if (Directory.Exists(secretPath))
        foreach (string secret in Directory.GetFiles(secretPath, $"*.json"))
            configHost.AddJsonFile(Path.GetFullPath(secret), optional: true, reloadOnChange: true);
    else
        logger.Warn("Секреты не найдены");

    configHost.AddEnvironmentVariables();
    configHost.AddCommandLine(args);
});

builder.ConfigureServices((context, services) =>
{
    services
    .Configure<RabbitMQConfigModel>(context.Configuration.GetSection("RabbitMQConfig"))
    .Configure<WebConfigModel>(context.Configuration.GetSection("WebConfig"))
    ;
    MongoConfigModel _jo = context.Configuration.GetSection("MongoDB").Get<MongoConfigModel>()!;
    services.AddSingleton(new MongoClient(_jo.ToString()).GetDatabase(_jo.FilesSystemName));

    services.AddMemoryCache();
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = context.Configuration.GetConnectionString("RedisConnectionString");
        // options.InstanceName = "app.";
    });
    services.AddSingleton<IManualCustomCacheService, ManualCustomCacheService>();

    services.AddOptions();

    string connectionIdentityString = context.Configuration.GetConnectionString("CloudParametersConnection") ?? throw new InvalidOperationException("Connection string 'CloudParametersConnection' not found.");
    services.AddDbContextFactory<StorageContext>(opt =>
    opt.UseNpgsql(connectionIdentityString));

    services
    .AddScoped<IHelpdeskRemoteTransmissionService, TransmissionHelpdeskService>()
    .AddScoped<ICommerceRemoteTransmissionService, TransmissionCommerceService>();

    #region MQ Transmission (remote methods call)
    services.AddScoped<IRabbitClient, RabbitClient>()
    .AddScoped<IWebRemoteTransmissionService, TransmissionWebService>()
    .AddScoped<ITelegramRemoteTransmissionService, TransmissionTelegramService>();

    services.AddScoped<ISerializeStorage, StorageServiceImpl>();
    ////
    services
    .RegisterMqListener<SaveParameterReceive, StorageCloudParameterPayloadModel?, int?>()
    .RegisterMqListener<SaveFileReceive, StorageImageMetadataModel?, StorageFileModelDB?>()
    .RegisterMqListener<TagSetReceive, TagSetModel?, bool?>()
    .RegisterMqListener<SetWebConfigReceive, WebConfigModel?, object?>()
    .RegisterMqListener<ReadFileReceive, TAuthRequestModel<RequestFileReadModel>?, StorageFileResponseModel?>()
    .RegisterMqListener<TagsSelectReceive, TPaginationRequestModel<SelectMetadataRequestModel>?, TPaginationResponseModel<TagModelDB>?>()
    .RegisterMqListener<FilesAreaGetMetadataReceive, FilesAreaMetadataRequestModel?, FilesAreaMetadataModel[]?>()
    .RegisterMqListener<FilesSelectReceive, TPaginationRequestModel<SelectMetadataRequestModel>?, TPaginationResponseModel<StorageFileModelDB>?>()
    .RegisterMqListener<ReadParameterReceive, StorageMetadataModel?, StorageCloudParameterPayloadModel?>()
    .RegisterMqListener<FindParametersReceive, RequestStorageBaseModel?, FoundParameterModel[]?>();
    //
    #endregion

});

IHost app = builder.Build();

using (IServiceScope ss = app.Services.CreateScope())
{
    IOptions<WebConfigModel> wc_main = ss.ServiceProvider.GetRequiredService<IOptions<WebConfigModel>>();
    IWebRemoteTransmissionService webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebRemoteTransmissionService>();
    TResponseModel<TelegramBotConfigModel?> wc_remote = await webRemoteCall.GetWebConfig();
    if (wc_remote.Response is not null && wc_remote.Success() && Uri.TryCreate(wc_remote.Response.BaseUri, UriKind.Absolute, out _))
        wc_main.Value.Update(wc_remote.Response.BaseUri);
}

await app.RunAsync();