////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Transmission.Receives.constructor;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using RemoteCallLib;
using SharedLib;
using NLog.Web;
using DbcLib;
using NLog;
using Microsoft.Extensions.Options;
using ConstructorService;

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
   .Configure<ConstructorConfigModel>(builder.Configuration.GetSection("ConstructorConfig"));

builder.Services.AddOptions();

string connectionIdentityString = builder.Configuration.GetConnectionString("ConstructorConnection") ?? throw new InvalidOperationException("Connection string 'ConstructorConnection' not found.");
builder.Services.AddDbContextFactory<ConstructorContext>(opt =>
{
    opt.UseNpgsql(connectionIdentityString);

#if DEBUG
    opt.EnableSensitiveDataLogging(true);
#endif
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IConstructorService, FormsConstructorService>();

#region MQ Transmission (remote methods call)
builder.Services.AddScoped<IRabbitClient, RabbitClient>();
//
builder.Services.AddScoped<IWebRemoteTransmissionService, TransmissionWebService>()
.AddScoped<ITelegramRemoteTransmissionService, TransmissionTelegramService>()
.AddScoped<IHelpdeskRemoteTransmissionService, TransmissionHelpdeskService>()
.AddScoped<ISerializeStorageRemoteTransmissionService, SerializeStorageRemoteTransmissionService>()
;
//
builder.Services
    .RegisterMqListener<CreateProjectReceive, CreateProjectRequestModel?, int?>()
    .RegisterMqListener<ProjectsReadReceive, int[]?, ProjectModelDb[]?>()

    .RegisterMqListener<CheckAndNormalizeSortIndexForElementsOfDirectoryReceive, int?, bool?>()
    .RegisterMqListener<AddRowToTableReceive, FieldSessionDocumentDataBaseModel?, int?>()
    .RegisterMqListener<DeleteValuesFieldsByGroupSessionDocumentDataByRowNumReceive, ValueFieldSessionDocumentDataBaseModel?, object?>()
    .RegisterMqListener<SetDoneSessionDocumentDataReceive, string?, object?>()
    .RegisterMqListener<SetValueFieldSessionDocumentDataReceive, SetValueFieldDocumentDataModel?, SessionOfDocumentDataModelDB?>()
    .RegisterMqListener<GetSessionDocumentDataReceive, string?, SessionOfDocumentDataModelDB?>()

    .RegisterMqListener<SelectFormsReceive, SelectFormsModel?, TPaginationResponseModel<FormConstructorModelDB>?>()
    .RegisterMqListener<DeleteSessionDocumentReceive, int?, object?>()
    .RegisterMqListener<ClearValuesForFieldNameReceive, FormFieldOfSessionModel?, object?>()
    .RegisterMqListener<FindSessionsDocumentsByFormFieldNameReceive, FormFieldModel?, EntryDictModel[]?>()
    .RegisterMqListener<RequestSessionsDocumentsReceive, RequestSessionsDocumentsRequestPaginationModel?, TPaginationResponseModel<SessionOfDocumentDataModelDB>?>()
    .RegisterMqListener<UpdateOrCreateSessionDocumentReceive, SessionOfDocumentDataModelDB?, SessionOfDocumentDataModelDB?>()
    .RegisterMqListener<GetSessionDocumentReceive, SessionGetModel?, SessionOfDocumentDataModelDB?>()
    .RegisterMqListener<SetStatusSessionDocumentReceive, SessionStatusModel?, object?>()
    .RegisterMqListener<SaveSessionFormReceive, SaveConstructorSessionRequestModel?, ValueDataForSessionOfDocumentModelDB[]?>()
    .RegisterMqListener<DeleteTabDocumentSchemeJoinFormReceive, TAuthRequestModel<int>?, object?>()
    .RegisterMqListener<MoveTabDocumentSchemeJoinFormReceive, TAuthRequestModel<MoveObjectModel>?, TabOfDocumentSchemeConstructorModelDB?>()
    .RegisterMqListener<CreateOrUpdateTabDocumentSchemeJoinFormReceive, TAuthRequestModel<FormToTabJoinConstructorModelDB>?, object?>()
    .RegisterMqListener<GetTabDocumentSchemeJoinFormReceive, int?, FormToTabJoinConstructorModelDB?>()
    .RegisterMqListener<DeleteTabOfDocumentSchemeReceive, TAuthRequestModel<int>?, object?>()
    .RegisterMqListener<GetTabOfDocumentSchemeReceive, int?, TabOfDocumentSchemeConstructorModelDB?>()
    .RegisterMqListener<MoveTabOfDocumentSchemeReceive, TAuthRequestModel<MoveObjectModel>?, DocumentSchemeConstructorModelDB?>()
    .RegisterMqListener<CreateOrUpdateTabOfDocumentSchemeReceive, TAuthRequestModel<EntryDescriptionOwnedModel>?, TabOfDocumentSchemeConstructorModelDB?>()
    .RegisterMqListener<DeleteDocumentSchemeReceive, TAuthRequestModel<int>?, object?>()
    .RegisterMqListener<GetDocumentSchemeReceive, int?, DocumentSchemeConstructorModelDB?>()
    .RegisterMqListener<RequestDocumentsSchemesReceive, RequestDocumentsSchemesModel?, TPaginationResponseModel<DocumentSchemeConstructorModelDB>?>()
    .RegisterMqListener<UpdateOrCreateDocumentSchemeReceive, TAuthRequestModel<EntryConstructedModel>?, DocumentSchemeConstructorModelDB?>()
    .RegisterMqListener<FormFieldDirectoryDeleteReceive, TAuthRequestModel<int>?, object?>()
    .RegisterMqListener<FormFieldDirectoryUpdateOrCreateReceive, TAuthRequestModel<FieldFormAkaDirectoryConstructorModelDB>?, object?>()
    .RegisterMqListener<FormFieldDeleteReceive, TAuthRequestModel<int>?, object?>()
    .RegisterMqListener<FormFieldUpdateOrCreateReceive, TAuthRequestModel<FieldFormConstructorModelDB>?, object?>()
    .RegisterMqListener<FieldDirectoryFormMoveReceive, TAuthRequestModel<MoveObjectModel>?, FormConstructorModelDB?>()
    .RegisterMqListener<FieldFormMoveReceive, TAuthRequestModel<MoveObjectModel>?, FormConstructorModelDB?>()
    .RegisterMqListener<FormDeleteReceive, TAuthRequestModel<int>?, object?>()
    .RegisterMqListener<FormUpdateOrCreateReceive, TAuthRequestModel<FormBaseConstructorModel>?, FormConstructorModelDB?>()
    .RegisterMqListener<GetFormReceive, int?, FormConstructorModelDB?>()
    .RegisterMqListener<GetElementsOfDirectoryReceive, int?, List<EntryModel>?>()
    .RegisterMqListener<CreateElementForDirectoryReceive, TAuthRequestModel<OwnedNameModel>?, int?>()
    .RegisterMqListener<UpdateElementOfDirectoryReceive, TAuthRequestModel<EntryDescriptionModel>?, object?>()
    .RegisterMqListener<GetElementOfDirectoryReceive, int?, EntryDescriptionModel?>()
    .RegisterMqListener<DeleteElementFromDirectoryReceive, TAuthRequestModel<int>?, object?>()
    .RegisterMqListener<UpMoveElementOfDirectoryReceive, TAuthRequestModel<int>?, object?>()
    .RegisterMqListener<DownMoveElementOfDirectoryReceive, TAuthRequestModel<int>?, object?>()
    .RegisterMqListener<DeleteDirectoryReceive, TAuthRequestModel<int>?, object?>()
    .RegisterMqListener<UpdateOrCreateDirectoryReceive, TAuthRequestModel<EntryConstructedModel>?, int?>()
    .RegisterMqListener<GetDirectoryReceive, int?, EntryDescriptionModel?>()
    .RegisterMqListener<GetDirectoriesReceive, ProjectFindModel?, EntryModel[]?>()
    .RegisterMqListener<ReadDirectoriesReceive, int[]?, EntryNestedModel[]?>()
    .RegisterMqListener<GetCurrentMainProjectReceive, string?, MainProjectViewModel?>()
    .RegisterMqListener<DeleteMembersFromProjectReceive, UsersProjectModel?, object?>()
    .RegisterMqListener<CanEditProjectReceive, UserProjectModel?, object?>()
    .RegisterMqListener<SetProjectAsMainReceive, UserProjectModel?, object?>()
    .RegisterMqListener<AddMembersToProjectReceive, UsersProjectModel?, object?>()
    .RegisterMqListener<GetMembersOfProjectReceive, int?, EntryAltModel[]?>()
    .RegisterMqListener<UpdateProjectReceive, ProjectViewModel?, object?>()
    .RegisterMqListener<SetMarkerDeleteProjectReceive, SetMarkerProjectRequestModel?, object?>()
    .RegisterMqListener<GetProjectsForUserReceive, GetProjectsForUserRequestModel?, ProjectViewModel[]?>()
    ;
#endregion

IHost host = builder.Build();

using (IServiceScope ss = host.Services.CreateScope())
{
    IOptions<ConstructorConfigModel> wc_main = ss.ServiceProvider.GetRequiredService<IOptions<ConstructorConfigModel>>();
    IWebRemoteTransmissionService webRemoteCall = ss.ServiceProvider.GetRequiredService<IWebRemoteTransmissionService>();
    TResponseModel<TelegramBotConfigModel?> wc_remote = await webRemoteCall.GetWebConfig();
    if (wc_remote.Response is not null && wc_remote.Success())
        wc_main.Value.WebConfig = wc_remote.Response;
}

host.Run();