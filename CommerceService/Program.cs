////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Transmission.Receives.commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Extensions.Logging;
using CommerceService;
using RemoteCallLib;
using SharedLib;
using NLog.Web;
using DbcLib;
using NLog;

// Early init of NLog to allow startup and exception logging, before host is built
Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
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
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
else
    logger.Warn($"отсутсвует: {path_load}");

path_load = Path.Combine(curr_dir, $"appsettings.{_environmentName}.json");
if (Path.Exists(path_load))
    builder.Configuration.AddJsonFile(path_load, optional: true, reloadOnChange: true);
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
;

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString($"RedisConnectionString{_modePrefix}");
    // options.InstanceName = "app.";
});

builder.Services.AddScoped<ICommerceService, CommerceImplementService>();

builder.Services.AddSingleton<WebConfigModel>();
builder.Services.AddOptions();
builder.Services.AddSingleton<IManualCustomCacheService, ManualCustomCacheService>();
string connectionIdentityString = builder.Configuration.GetConnectionString($"CommerceConnection{_modePrefix}") ?? throw new InvalidOperationException($"Connection string 'CommerceConnection{_modePrefix}' not found.");
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
    .RegisterMqListener<OffersRegistersSelectReceive, TPaginationRequestModel<RegistersSelectRequestBaseModel>?, TPaginationResponseModel<OfferAvailabilityModelDB>?>()
    .RegisterMqListener<WarehousesSelectReceive, TPaginationRequestModel<WarehouseDocumentsSelectRequestModel>?, TPaginationResponseModel<WarehouseDocumentModelDB>?>()
    .RegisterMqListener<WarehousesDocumentsReadReceive, int[]?, WarehouseDocumentModelDB[]?>()
    .RegisterMqListener<WarehouseDocumentUpdateReceive, WarehouseDocumentModelDB?, int?>()
    .RegisterMqListener<RowsForWarehouseDocumentDeleteReceive, int[]?, bool?>()
    .RegisterMqListener<RowForWarehouseDocumentUpdateReceive, RowOfWarehouseDocumentModelDB?, int?>()
    .RegisterMqListener<StatusChangeReceive, StatusOrderChangeRequestModel?, bool?>()
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