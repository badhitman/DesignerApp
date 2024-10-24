////////////////////////////////////////////////
// � https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Transmission.Receives.commerce;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using RemoteCallLib;
using SharedLib;
using NLog.Web;
using DbcLib;
using NLog;
using MongoDB.Driver;
using DbLayerLib;

// Early init of NLog to allow startup and exception logging, before host is built
Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Warn("init main");
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

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

MongoConfigModel _jo = builder.Configuration.GetSection("MongoDB").Get<MongoConfigModel>()!;
builder.Services.AddSingleton(new MongoClient(_jo.ToString()).GetDatabase(_jo.FilesSystemName));

builder.Services.AddSingleton<WebConfigModel>();
builder.Services.AddOptions();

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
    .RegisterMqListener<GetFileOrderReceive, string?, byte[]?>()
    .RegisterMqListener<PriceRuleDeleteReceive, int?, bool?>()
    .RegisterMqListener<PriceRuleUpdateReceive, PriceRuleForOfferModelDB?, int?>()
    .RegisterMqListener<PricesRulesGetForOffersReceive, int[]?, PriceRuleForOfferModelDB[]?>()
    .RegisterMqListener<PaymentDocumentUpdateReceive, PaymentDocumentBaseModel?, int?>()
    .RegisterMqListener<OrderUpdateReceive, OrderDocumentModelDB?, int?>()
    .RegisterMqListener<OffersReadReceive, int[]?, OfferGoodModelDB[]?>()
    .RegisterMqListener<GoodsReadReceive, int[]?, GoodsModelDB[]?>()
    .RegisterMqListener<AddressesOrganizationsReadReceive, int[]?, AddressOrganizationModelDB[]?>()
    .RegisterMqListener<AttachmentDeleteFromOrderReceive, int?, bool?>()
    .RegisterMqListener<AttachmentForOrderReceive, AttachmentForOrderRequestModel?, int?>()
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