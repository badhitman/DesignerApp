////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Transmission.Receives.constructor;
using SharedLib;

namespace ConstructorService;

/// <summary>
/// MQ listen
/// </summary>
public static class RegisterMqListenerExtension
{
    /// <summary>
    /// RegisterMqListeners
    /// </summary>
    public static IServiceCollection ConstructorRegisterMqListeners(this IServiceCollection services)
    {
        return services
            .RegisterMqListener<CreateProjectReceive, CreateProjectRequestModel, TResponseModel<int>>()
            .RegisterMqListener<ProjectsReadReceive, int[], List<ProjectModelDb>>()
            .RegisterMqListener<CheckAndNormalizeSortIndexForElementsOfDirectoryReceive, int, ResponseBaseModel>()
            .RegisterMqListener<AddRowToTableReceive, FieldSessionDocumentDataBaseModel, TResponseModel<int>>()
            .RegisterMqListener<DeleteValuesFieldsByGroupSessionDocumentDataByRowNumReceive, ValueFieldSessionDocumentDataBaseModel, ResponseBaseModel>()
            .RegisterMqListener<SetDoneSessionDocumentDataReceive, string, ResponseBaseModel>()
            .RegisterMqListener<SetValueFieldSessionDocumentDataReceive, SetValueFieldDocumentDataModel, TResponseModel<SessionOfDocumentDataModelDB>>()
            .RegisterMqListener<GetSessionDocumentDataReceive, string, TResponseModel<SessionOfDocumentDataModelDB>>()
            .RegisterMqListener<SelectFormsReceive, SelectFormsModel, TPaginationResponseModel<FormConstructorModelDB>>()
            .RegisterMqListener<DeleteSessionDocumentReceive, int, ResponseBaseModel>()
            .RegisterMqListener<ClearValuesForFieldNameReceive, FormFieldOfSessionModel, ResponseBaseModel>()
            .RegisterMqListener<FindSessionsDocumentsByFormFieldNameReceive, FormFieldModel, TResponseModel<EntryDictModel[]>>()
            .RegisterMqListener<RequestSessionsDocumentsReceive, RequestSessionsDocumentsRequestPaginationModel, TPaginationResponseModel<SessionOfDocumentDataModelDB>>()
            .RegisterMqListener<UpdateOrCreateSessionDocumentReceive, SessionOfDocumentDataModelDB, TResponseModel<SessionOfDocumentDataModelDB>>()
            .RegisterMqListener<GetSessionDocumentReceive, SessionGetModel, TResponseModel<SessionOfDocumentDataModelDB>>()
            .RegisterMqListener<SetStatusSessionDocumentReceive, SessionStatusModel, ResponseBaseModel>()
            .RegisterMqListener<SaveSessionFormReceive, SaveConstructorSessionRequestModel, TResponseModel<ValueDataForSessionOfDocumentModelDB[]>>()
            .RegisterMqListener<DeleteTabDocumentSchemeJoinFormReceive, TAuthRequestModel<int>, ResponseBaseModel>()
            .RegisterMqListener<MoveTabDocumentSchemeJoinFormReceive, TAuthRequestModel<MoveObjectModel>, TResponseModel<TabOfDocumentSchemeConstructorModelDB>>()
            .RegisterMqListener<CreateOrUpdateTabDocumentSchemeJoinFormReceive, TAuthRequestModel<FormToTabJoinConstructorModelDB>, ResponseBaseModel>()
            .RegisterMqListener<GetTabDocumentSchemeJoinFormReceive, int, TResponseModel<FormToTabJoinConstructorModelDB>>()
            .RegisterMqListener<DeleteTabOfDocumentSchemeReceive, TAuthRequestModel<int>, ResponseBaseModel>()
            .RegisterMqListener<GetTabOfDocumentSchemeReceive, int, TResponseModel<TabOfDocumentSchemeConstructorModelDB>>()
            .RegisterMqListener<MoveTabOfDocumentSchemeReceive, TAuthRequestModel<MoveObjectModel>, TResponseModel<DocumentSchemeConstructorModelDB>>()
            .RegisterMqListener<CreateOrUpdateTabOfDocumentSchemeReceive, TAuthRequestModel<EntryDescriptionOwnedModel>, TResponseModel<TabOfDocumentSchemeConstructorModelDB>>()
            .RegisterMqListener<DeleteDocumentSchemeReceive, TAuthRequestModel<int>, ResponseBaseModel>()
            .RegisterMqListener<GetDocumentSchemeReceive, int, TResponseModel<DocumentSchemeConstructorModelDB>>()
            .RegisterMqListener<RequestDocumentsSchemesReceive, RequestDocumentsSchemesModel, TPaginationResponseModel<DocumentSchemeConstructorModelDB>>()
            .RegisterMqListener<UpdateOrCreateDocumentSchemeReceive, TAuthRequestModel<EntryConstructedModel>, TResponseModel<DocumentSchemeConstructorModelDB>>()
            .RegisterMqListener<FormFieldDirectoryDeleteReceive, TAuthRequestModel<int>, ResponseBaseModel>()
            .RegisterMqListener<FormFieldDirectoryUpdateOrCreateReceive, TAuthRequestModel<FieldFormAkaDirectoryConstructorModelDB>, ResponseBaseModel>()
            .RegisterMqListener<FormFieldDeleteReceive, TAuthRequestModel<int>, ResponseBaseModel>()
            .RegisterMqListener<FormFieldUpdateOrCreateReceive, TAuthRequestModel<FieldFormConstructorModelDB>, ResponseBaseModel>()
            .RegisterMqListener<FieldDirectoryFormMoveReceive, TAuthRequestModel<MoveObjectModel>, TResponseModel<FormConstructorModelDB>>()
            .RegisterMqListener<FieldFormMoveReceive, TAuthRequestModel<MoveObjectModel>, TResponseModel<FormConstructorModelDB>>()
            .RegisterMqListener<FormDeleteReceive, TAuthRequestModel<int>, ResponseBaseModel>()
            .RegisterMqListener<FormUpdateOrCreateReceive, TAuthRequestModel<FormBaseConstructorModel>, TResponseModel<FormConstructorModelDB>>()
            .RegisterMqListener<GetFormReceive, int, TResponseModel<FormConstructorModelDB>>()
            .RegisterMqListener<GetElementsOfDirectoryReceive, int, TResponseModel<List<EntryModel>>>()
            .RegisterMqListener<CreateElementForDirectoryReceive, TAuthRequestModel<OwnedNameModel>, TResponseModel<int>>()
            .RegisterMqListener<UpdateElementOfDirectoryReceive, TAuthRequestModel<EntryDescriptionModel>, ResponseBaseModel>()
            .RegisterMqListener<GetElementOfDirectoryReceive, int, TResponseModel<EntryDescriptionModel>>()
            .RegisterMqListener<DeleteElementFromDirectoryReceive, TAuthRequestModel<int>, ResponseBaseModel>()
            .RegisterMqListener<UpMoveElementOfDirectoryReceive, TAuthRequestModel<int>, ResponseBaseModel>()
            .RegisterMqListener<DownMoveElementOfDirectoryReceive, TAuthRequestModel<int>, ResponseBaseModel>()
            .RegisterMqListener<DeleteDirectoryReceive, TAuthRequestModel<int>, ResponseBaseModel>()
            .RegisterMqListener<UpdateOrCreateDirectoryReceive, TAuthRequestModel<EntryConstructedModel>, TResponseModel<int>>()
            .RegisterMqListener<GetDirectoryReceive, int, TResponseModel<EntryDescriptionModel>>()
            .RegisterMqListener<GetDirectoriesReceive, ProjectFindModel, TResponseModel<EntryModel[]>>()
            .RegisterMqListener<ReadDirectoriesReceive, int[], List<EntryNestedModel>>()
            .RegisterMqListener<GetCurrentMainProjectReceive, string, TResponseModel<MainProjectViewModel>>()
            .RegisterMqListener<DeleteMembersFromProjectReceive, UsersProjectModel, ResponseBaseModel>()
            .RegisterMqListener<CanEditProjectReceive, UserProjectModel, ResponseBaseModel>()
            .RegisterMqListener<SetProjectAsMainReceive, UserProjectModel, ResponseBaseModel>()
            .RegisterMqListener<AddMembersToProjectReceive, UsersProjectModel, ResponseBaseModel>()
            .RegisterMqListener<GetMembersOfProjectReceive, int, TResponseModel<EntryAltModel[]>>()
            .RegisterMqListener<UpdateProjectReceive, ProjectViewModel, ResponseBaseModel>()
            .RegisterMqListener<SetMarkerDeleteProjectReceive, SetMarkerProjectRequestModel, ResponseBaseModel>()
            .RegisterMqListener<GetProjectsForUserReceive, GetProjectsForUserRequestModel, TResponseModel<ProjectViewModel[]>>()
            ;
    }
}