////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Transmission.Receives.commerce;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using RemoteCallLib;
using SharedLib;
using NLog.Web;
using DbcLib;
using NLog;
using Microsoft.AspNetCore.Hosting;

// Early init of NLog to allow startup and exception logging, before host is built
Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

IHostEnvironment env = builder.Environment;
logger.Warn($"init main: {env.EnvironmentName}");

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
;

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
    // options.InstanceName = "app.";
});

builder.Services.AddHttpClient(HttpClientsNamesEnum.Wappi.ToString(), cc =>
{
    cc.BaseAddress = new Uri("https://wappi.pro/");
});

builder.Services.AddSingleton<WebConfigModel>();
builder.Services.AddOptions();
builder.Services.AddSingleton<IManualCustomCacheService, ManualCustomCacheService>();
string connectionIdentityString = builder.Configuration.GetConnectionString("CommerceConnection") ?? throw new InvalidOperationException("Connection string 'CommerceConnection' not found.");
builder.Services.AddDbContextFactory<CommerceContext>(opt =>
{
    opt.UseNpgsql(connectionIdentityString);

#if DEBUG
    opt.EnableSensitiveDataLogging(true);
#endif
});

builder.Services.AddMemoryCache();

#region MQ Transmission (remote methods call)
builder.Services.AddScoped<IRabbitClient, RabbitClient>();
//
builder.Services.AddScoped<IWebRemoteTransmissionService, TransmissionWebService>()
.AddScoped<ITelegramRemoteTransmissionService, TransmissionTelegramService>()
.AddScoped<IHelpdeskRemoteTransmissionService, TransmissionHelpdeskService>()
.AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>();
//
builder.Services
    .RegisterMqListener<OrganizationSetLegalReceive, OrganizationLegalModel?, bool?>()
    .RegisterMqListener<OrganizationUpdateReceive, TAuthRequestModel<OrganizationModelDB>?, int?>()
    .RegisterMqListener<OrganizationsSelectReceive, TPaginationRequestModel<OrganizationsSelectRequestModel>?, TPaginationResponseModel<OrganizationModelDB>?>()
    .RegisterMqListener<AddressOrganizationUpdateReceive, AddressOrganizationBaseModel?, int?>()
    .RegisterMqListener<AddressOrganizationDeleteReceive, int?, bool?>()
    .RegisterMqListener<GoodsUpdateReceive, GoodsModelDB?, int?>()
    .RegisterMqListener<OrdersByIssuesGetReceive, OrdersByIssuesSelectRequestModel?, OrderDocumentModelDB[]?>()
    .RegisterMqListener<OfferDeleteReceive, int?, bool?>()
    .RegisterMqListener<StatusChangeReceive, StatusChangeRequestModel?, bool?>()
    .RegisterMqListener<PriceRuleDeleteReceive, int?, bool?>()
    .RegisterMqListener<PriceRuleUpdateReceive, PriceRuleForOfferModelDB?, int?>()
    .RegisterMqListener<PricesRulesGetForOffersReceive, int[]?, PriceRuleForOfferModelDB[]?>()
    .RegisterMqListener<PaymentDocumentUpdateReceive, PaymentDocumentBaseModel?, int?>()
    .RegisterMqListener<OrderUpdateReceive, OrderDocumentModelDB?, int?>()
    .RegisterMqListener<OffersReadReceive, int[]?, OfferGoodModelDB[]?>()
    .RegisterMqListener<GoodsReadReceive, int[]?, GoodsModelDB[]?>()
    .RegisterMqListener<AddressesOrganizationsReadReceive, int[]?, AddressOrganizationModelDB[]?>()
    .RegisterMqListener<PaymentDocumentDeleteReceive, int?, bool?>()
    .RegisterMqListener<RowsForOrderDeleteReceive, int[]?, bool?>()
    .RegisterMqListener<RowForOrderUpdateReceive, RowOfOrderDocumentModelDB?, int?>()
    .RegisterMqListener<OrdersReadReceive, int[]?, OrderDocumentModelDB[]?>()
    .RegisterMqListener<OrdersSelectReceive, TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>>?, TPaginationResponseModel<OrderDocumentModelDB>?>()
    .RegisterMqListener<OfferUpdateReceive, OfferGoodModelDB?, int?>()
    .RegisterMqListener<OffersSelectReceive, TPaginationRequestModel<OffersSelectRequestModel>?, TPaginationResponseModel<OfferGoodModelDB>?>()
    .RegisterMqListener<GoodsSelectReceive, TPaginationRequestModel<GoodsSelectRequestModel>?, TPaginationResponseModel<GoodsModelDB>?>()
    .RegisterMqListener<OrganizationsReadReceive, int[]?, OrganizationModelDB[]?>();
#endregion

IHost host = builder.Build();

host.Run();