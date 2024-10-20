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
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.AddRowToTableReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> DeleteValuesFieldsByGroupSessionDocumentDataByRowNum(ValueFieldSessionDocumentDataBaseModel req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.DeleteValuesFieldsByGroupSessionDocumentDataByRowNumReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> SetDoneSessionDocumentData(string req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.SetDoneSessionDocumentDataReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> SetValueFieldSessionDocumentData(SetValueFieldDocumentDataModel req)
        => await rabbitClient.MqRemoteCall<SessionOfDocumentDataModelDB>(GlobalStaticConstants.TransmissionQueues.SetValueFieldSessionDocumentDataReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionDocumentData(string req)
        => await rabbitClient.MqRemoteCall<SessionOfDocumentDataModelDB>(GlobalStaticConstants.TransmissionQueues.GetSessionDocumentDataReceive, req);
    #endregion

    #region projects
    /// <inheritdoc/>
    public async Task<TResponseModel<object>> AddMembersToProject(UsersProjectModel req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.AddMembersToProjectReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<ProjectViewModel[]>> GetProjectsForUser(GetProjectsForUserRequestModel req)
        => await rabbitClient.MqRemoteCall<ProjectViewModel[]>(GlobalStaticConstants.TransmissionQueues.ProjectsForUserReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<ProjectModelDb[]>> ProjectsRead(int[] ids)
        => await rabbitClient.MqRemoteCall<ProjectModelDb[]>(GlobalStaticConstants.TransmissionQueues.ProjectsReadReceive, ids);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> SetMarkerDeleteProject(SetMarkerProjectRequestModel req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.SetMarkerDeleteProjectReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> UpdateProject(ProjectViewModel req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.UpdateProjectReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> SetProjectAsMain(UserProjectModel req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.SetProjectAsMainReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<MainProjectViewModel>> GetCurrentMainProject(string req)
        => await rabbitClient.MqRemoteCall<MainProjectViewModel>(GlobalStaticConstants.TransmissionQueues.GetCurrentMainProjectReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CreateProject(CreateProjectRequestModel req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.ProjectCreateReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryAltModel[]>> GetMembersOfProject(int req)
        => await rabbitClient.MqRemoteCall<EntryAltModel[]>(GlobalStaticConstants.TransmissionQueues.GetMembersOfProjectReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> DeleteMembersFromProject(UsersProjectModel req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.DeleteMembersFromProjectReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> CanEditProject(UserProjectModel req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.CanEditProjectReceive, req);
    #endregion

    #region directories
    /// <inheritdoc/>
    public async Task<TResponseModel<EntryModel[]>> GetDirectories(ProjectFindModel req)
        => await rabbitClient.MqRemoteCall<EntryModel[]>(GlobalStaticConstants.TransmissionQueues.GetDirectoriesReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDescriptionModel>> GetDirectory(int req)
        => await rabbitClient.MqRemoteCall<EntryDescriptionModel>(GlobalStaticConstants.TransmissionQueues.GetDirectoryReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryNestedModel[]>> ReadDirectories(int[] req)
        => await rabbitClient.MqRemoteCall<EntryNestedModel[]>(GlobalStaticConstants.TransmissionQueues.ReadDirectoriesReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> UpdateOrCreateDirectory(TAuthRequestModel<EntryConstructedModel> req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.UpdateOrCreateDirectoryReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> DeleteDirectory(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.DeleteDirectoryReceive, req);
    #endregion
    #region elements of directories
    /// <inheritdoc/>
    public async Task<TResponseModel<List<EntryModel>>> GetElementsOfDirectory(int req)
        => await rabbitClient.MqRemoteCall<List<EntryModel>>(GlobalStaticConstants.TransmissionQueues.GetElementsOfDirectoryReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CreateElementForDirectory(TAuthRequestModel<OwnedNameModel> req)
        => await rabbitClient.MqRemoteCall<int>(GlobalStaticConstants.TransmissionQueues.CreateElementForDirectoryReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> UpdateElementOfDirectory(TAuthRequestModel<EntryDescriptionModel> req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.UpdateElementOfDirectoryReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDescriptionModel>> GetElementOfDirectory(int req)
        => await rabbitClient.MqRemoteCall<EntryDescriptionModel>(GlobalStaticConstants.TransmissionQueues.GetElementOfDirectoryReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> DeleteElementFromDirectory(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.DeleteElementFromDirectoryReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> UpMoveElementOfDirectory(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.UpMoveElementOfDirectoryReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> DownMoveElementOfDirectory(TAuthRequestModel<int> req)
        => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.DownMoveElementOfDirectoryReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> CheckAndNormalizeSortIndexForElementsOfDirectory(int req)
        => await rabbitClient.MqRemoteCall<bool>(GlobalStaticConstants.TransmissionQueues.CheckAndNormalizeSortIndexForElementsOfDirectoryReceive, req);
    #endregion

    #region forms
    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<FormConstructorModelDB>>> SelectForms(SelectFormsModel req, CancellationToken cancellationToken = default)
    => await rabbitClient.MqRemoteCall<TPaginationResponseModel<FormConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.SelectFormsReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> GetForm(int req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<FormConstructorModelDB>(GlobalStaticConstants.TransmissionQueues.GetFormReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> FormUpdateOrCreate(TAuthRequestModel<FormBaseConstructorModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<FormConstructorModelDB>(GlobalStaticConstants.TransmissionQueues.FormUpdateOrCreateReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> FormDelete(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.FormDeleteReceive, req);
    #endregion
    #region fiealds
    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> FieldFormMove(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<FormConstructorModelDB>(GlobalStaticConstants.TransmissionQueues.FieldFormMoveReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> FieldDirectoryFormMove(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<FormConstructorModelDB>(GlobalStaticConstants.TransmissionQueues.FieldDirectoryFormMoveReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> FormFieldUpdateOrCreate(TAuthRequestModel<FieldFormConstructorModelDB> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.FormFieldUpdateOrCreateReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> FormFieldDelete(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.FormFieldDeleteReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> FormFieldDirectoryUpdateOrCreate(TAuthRequestModel<FieldFormAkaDirectoryConstructorModelDB> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.FormFieldDirectoryUpdateOrCreateReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> FormFieldDirectoryDelete(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.FormFieldDirectoryDeleteReceive, req);
    #endregion

    #region documents
    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB>> UpdateOrCreateDocumentScheme(TAuthRequestModel<EntryConstructedModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<DocumentSchemeConstructorModelDB>(GlobalStaticConstants.TransmissionQueues.UpdateOrCreateDocumentSchemeReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<DocumentSchemeConstructorModelDB>>> RequestDocumentsSchemes(RequestDocumentsSchemesModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TPaginationResponseModel<DocumentSchemeConstructorModelDB>>(GlobalStaticConstants.TransmissionQueues.RequestDocumentsSchemesReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB>> GetDocumentScheme(int req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<DocumentSchemeConstructorModelDB>(GlobalStaticConstants.TransmissionQueues.GetDocumentSchemeReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> DeleteDocumentScheme(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.DeleteDocumentSchemeReceive, req);
    #endregion
    #region табы документов
    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> CreateOrUpdateTabOfDocumentScheme(TAuthRequestModel<EntryDescriptionOwnedModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TabOfDocumentSchemeConstructorModelDB>(GlobalStaticConstants.TransmissionQueues.CreateOrUpdateTabOfDocumentSchemeReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB>> MoveTabOfDocumentScheme(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<DocumentSchemeConstructorModelDB>(GlobalStaticConstants.TransmissionQueues.MoveTabOfDocumentSchemeReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> GetTabOfDocumentScheme(int req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TabOfDocumentSchemeConstructorModelDB>(GlobalStaticConstants.TransmissionQueues.GetTabOfDocumentSchemeReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> DeleteTabOfDocumentScheme(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.DeleteTabOfDocumentSchemeReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<FormToTabJoinConstructorModelDB>> GetTabDocumentSchemeJoinForm(int req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<FormToTabJoinConstructorModelDB>(GlobalStaticConstants.TransmissionQueues.GetTabDocumentSchemeJoinFormReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> CreateOrUpdateTabDocumentSchemeJoinForm(TAuthRequestModel<FormToTabJoinConstructorModelDB> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.CreateOrUpdateTabDocumentSchemeJoinFormReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> MoveTabDocumentSchemeJoinForm(TAuthRequestModel<MoveObjectModel> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TabOfDocumentSchemeConstructorModelDB>(GlobalStaticConstants.TransmissionQueues.MoveTabDocumentSchemeJoinFormReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> DeleteTabDocumentSchemeJoinForm(TAuthRequestModel<int> req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.DeleteTabDocumentSchemeJoinFormReceive, req);
    #endregion

    #region session
    /// <inheritdoc/>
    public async Task<TResponseModel<ValueDataForSessionOfDocumentModelDB[]>> SaveSessionForm(SaveConstructorSessionRequestModel req)
     => await rabbitClient.MqRemoteCall<ValueDataForSessionOfDocumentModelDB[]>(GlobalStaticConstants.TransmissionQueues.SaveSessionFormReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> SetStatusSessionDocument(SessionStatusModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.SetStatusSessionDocumentReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionDocument(SessionGetModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<SessionOfDocumentDataModelDB>(GlobalStaticConstants.TransmissionQueues.GetSessionDocumentReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> UpdateOrCreateSessionDocument(SessionOfDocumentDataModelDB req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<SessionOfDocumentDataModelDB>(GlobalStaticConstants.TransmissionQueues.UpdateOrCreateSessionDocumentReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<SessionOfDocumentDataModelDB>>> RequestSessionsDocuments(RequestSessionsDocumentsRequestPaginationModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<TPaginationResponseModel<SessionOfDocumentDataModelDB>>(GlobalStaticConstants.TransmissionQueues.RequestSessionsDocumentsReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDictModel[]>> FindSessionsDocumentsByFormFieldName(FormFieldModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<EntryDictModel[]>(GlobalStaticConstants.TransmissionQueues.FindSessionsDocumentsByFormFieldNameReceive, req);
    
    /// <inheritdoc/>
    public async Task<TResponseModel<object>> ClearValuesForFieldName(FormFieldOfSessionModel req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.ClearValuesForFieldNameReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<object>> DeleteSessionDocument(int req, CancellationToken cancellationToken = default)
     => await rabbitClient.MqRemoteCall<object>(GlobalStaticConstants.TransmissionQueues.DeleteSessionDocumentReceive, req);
    #endregion
}