////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// TransmissionConstructorService
/// </summary>
public class TransmissionConstructorService(IRabbitClient rabbitClient) : IConstructorRemoteTransmissionService
{
    #region public
    /// <inheritdoc/>
    public async Task<TResponseModel<int>> AddRowToTable(FieldSessionDocumentDataBaseModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.AddRowToTableReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteValuesFieldsByGroupSessionDocumentDataByRowNum(ValueFieldSessionDocumentDataBaseModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.DeleteValuesFieldsByGroupSessionDocumentDataByRowNumReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetDoneSessionDocumentData(string req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SetDoneSessionDocumentDataReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> SetValueFieldSessionDocumentData(SetValueFieldDocumentDataModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<SessionOfDocumentDataModelDB>>(GlobalStaticConstants.TransmissionQueues.SetValueFieldSessionDocumentDataReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionDocumentData(string req)
        => await rabbitClient.MqRemoteCall<TResponseModel<SessionOfDocumentDataModelDB>>(GlobalStaticConstants.TransmissionQueues.GetSessionDocumentDataReceive, req) ?? new();
    #endregion

    #region projects
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddMembersToProject(UsersProjectModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.AddMembersToProjectReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<ProjectViewModel[]>> GetProjectsForUser(GetProjectsForUserRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<ProjectViewModel[]>>(GlobalStaticConstants.TransmissionQueues.ProjectsForUserReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<List<ProjectModelDb>> ProjectsRead(int[] ids)
        => await rabbitClient.MqRemoteCall<List<ProjectModelDb>>(GlobalStaticConstants.TransmissionQueues.ProjectsReadReceive, ids) ?? [];

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetMarkerDeleteProject(SetMarkerProjectRequestModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SetMarkerDeleteProjectReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateProject(ProjectViewModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.UpdateProjectReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetProjectAsMain(UserProjectModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SetProjectAsMainReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<MainProjectViewModel>> GetCurrentMainProject(string req)
        => await rabbitClient.MqRemoteCall<TResponseModel<MainProjectViewModel>>(GlobalStaticConstants.TransmissionQueues.GetCurrentMainProjectReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CreateProject(CreateProjectRequestModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.ProjectCreateReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryAltModel[]>> GetMembersOfProject(int req)
        => await rabbitClient.MqRemoteCall<TResponseModel<EntryAltModel[]>>(GlobalStaticConstants.TransmissionQueues.GetMembersOfProjectReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteMembersFromProject(UsersProjectModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.DeleteMembersFromProjectReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CanEditProject(UserProjectModel req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.CanEditProjectReceive, req) ?? new();
    #endregion

    #region directories
    /// <inheritdoc/>
    public async Task<TResponseModel<EntryModel[]>> GetDirectories(ProjectFindModel req)
        => await rabbitClient.MqRemoteCall<TResponseModel<EntryModel[]>>(GlobalStaticConstants.TransmissionQueues.GetDirectoriesReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDescriptionModel>> GetDirectory(int req)
        => await rabbitClient.MqRemoteCall<TResponseModel<EntryDescriptionModel>>(GlobalStaticConstants.TransmissionQueues.GetDirectoryReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<List<EntryNestedModel>> ReadDirectories(int[] req)
        => await rabbitClient.MqRemoteCall<List<EntryNestedModel>>(GlobalStaticConstants.TransmissionQueues.ReadDirectoriesReceive, req) ?? [];

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> UpdateOrCreateDirectory(TAuthRequestModel<EntryConstructedModel> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.UpdateOrCreateDirectoryReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteDirectory(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.DeleteDirectoryReceive, req) ?? new();
    #endregion
    #region elements of directories
    /// <inheritdoc/>
    public async Task<TResponseModel<List<EntryModel>>> GetElementsOfDirectory(int req)
        => await rabbitClient.MqRemoteCall<TResponseModel<List<EntryModel>>>(GlobalStaticConstants.TransmissionQueues.GetElementsOfDirectoryReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CreateElementForDirectory(TAuthRequestModel<OwnedNameModel> req)
        => await rabbitClient.MqRemoteCall<TResponseModel<int>>(GlobalStaticConstants.TransmissionQueues.CreateElementForDirectoryReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateElementOfDirectory(TAuthRequestModel<EntryDescriptionModel> req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.UpdateElementOfDirectoryReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDescriptionModel>> GetElementOfDirectory(int req)
        => await rabbitClient.MqRemoteCall<TResponseModel<EntryDescriptionModel>>(GlobalStaticConstants.TransmissionQueues.GetElementOfDirectoryReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteElementFromDirectory(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.DeleteElementFromDirectoryReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpMoveElementOfDirectory(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.UpMoveElementOfDirectoryReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DownMoveElementOfDirectory(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.DownMoveElementOfDirectoryReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CheckAndNormalizeSortIndexForElementsOfDirectory(int req)
        => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.CheckAndNormalizeSortIndexForElementsOfDirectoryReceive, req) ?? new();
    #endregion

    #region forms
    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<FormConstructorModelDB>> SelectForms(SelectFormsModel req, CancellationToken cancellationToken = default)
    => await rabbitClient.MqRemoteCall<TPaginationResponseModel<FormConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.SelectFormsReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> GetForm(int req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<FormConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.GetFormReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> FormUpdateOrCreate(TAuthRequestModel<FormBaseConstructorModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<FormConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.FormUpdateOrCreateReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormDelete(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.FormDeleteReceive, req) ?? new();
    #endregion
    #region fiealds
    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> FieldFormMove(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<FormConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.FieldFormMoveReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> FieldDirectoryFormMove(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<FormConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.FieldDirectoryFormMoveReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldUpdateOrCreate(TAuthRequestModel<FieldFormConstructorModelDB> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.FormFieldUpdateOrCreateReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldDelete(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.FormFieldDeleteReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldDirectoryUpdateOrCreate(TAuthRequestModel<FieldFormAkaDirectoryConstructorModelDB> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.FormFieldDirectoryUpdateOrCreateReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldDirectoryDelete(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.FormFieldDirectoryDeleteReceive, req) ?? new();
    #endregion

    #region documents
    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB>> UpdateOrCreateDocumentScheme(TAuthRequestModel<EntryConstructedModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<DocumentSchemeConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.UpdateOrCreateDocumentSchemeReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<DocumentSchemeConstructorModelDB>> RequestDocumentsSchemes(RequestDocumentsSchemesModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TPaginationResponseModel<DocumentSchemeConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.RequestDocumentsSchemesReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB>> GetDocumentScheme(int req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<DocumentSchemeConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.GetDocumentSchemeReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteDocumentScheme(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.DeleteDocumentSchemeReceive, req) ?? new();
    #endregion
    #region табы документов
    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> CreateOrUpdateTabOfDocumentScheme(TAuthRequestModel<EntryDescriptionOwnedModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<TabOfDocumentSchemeConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.CreateOrUpdateTabOfDocumentSchemeReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB>> MoveTabOfDocumentScheme(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<DocumentSchemeConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.MoveTabOfDocumentSchemeReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> GetTabOfDocumentScheme(int req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<TabOfDocumentSchemeConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.GetTabOfDocumentSchemeReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteTabOfDocumentScheme(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.DeleteTabOfDocumentSchemeReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<FormToTabJoinConstructorModelDB>> GetTabDocumentSchemeJoinForm(int req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<FormToTabJoinConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.GetTabDocumentSchemeJoinFormReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CreateOrUpdateTabDocumentSchemeJoinForm(TAuthRequestModel<FormToTabJoinConstructorModelDB> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.CreateOrUpdateTabDocumentSchemeJoinFormReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> MoveTabDocumentSchemeJoinForm(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<TabOfDocumentSchemeConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.MoveTabDocumentSchemeJoinFormReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteTabDocumentSchemeJoinForm(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.DeleteTabDocumentSchemeJoinFormReceive, req) ?? new();
    #endregion

    #region session
    /// <inheritdoc/>
    public async Task<TResponseModel<ValueDataForSessionOfDocumentModelDB[]>> SaveSessionForm(SaveConstructorSessionRequestModel req)
     => await rabbitClient.MqRemoteCall<TResponseModel<ValueDataForSessionOfDocumentModelDB[]>>(GlobalStaticConstants.TransmissionQueues.SaveSessionFormReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetStatusSessionDocument(SessionStatusModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.SetStatusSessionDocumentReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionDocument(SessionGetModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<SessionOfDocumentDataModelDB>>(GlobalStaticConstants.TransmissionQueues.GetSessionDocumentReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> UpdateOrCreateSessionDocument(SessionOfDocumentDataModelDB req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<SessionOfDocumentDataModelDB>>(GlobalStaticConstants.TransmissionQueues.UpdateOrCreateSessionDocumentReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<SessionOfDocumentDataModelDB>> RequestSessionsDocuments(RequestSessionsDocumentsRequestPaginationModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TPaginationResponseModel<SessionOfDocumentDataModelDB>>(GlobalStaticConstants.TransmissionQueues.RequestSessionsDocumentsReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDictModel[]>> FindSessionsDocumentsByFormFieldName(FormFieldModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TResponseModel<EntryDictModel[]>>(GlobalStaticConstants.TransmissionQueues.FindSessionsDocumentsByFormFieldNameReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ClearValuesForFieldName(FormFieldOfSessionModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.ClearValuesForFieldNameReceive, req) ?? new();

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteSessionDocument(int req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<ResponseBaseModel>(GlobalStaticConstants.TransmissionQueues.DeleteSessionDocumentReceive, req) ?? new();
    #endregion
}