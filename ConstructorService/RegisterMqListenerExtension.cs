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
    }
}