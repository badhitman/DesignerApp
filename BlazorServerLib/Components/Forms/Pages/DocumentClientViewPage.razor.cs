////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Pages;

/// <summary>
/// 
/// </summary>
public partial class DocumentClientViewPage : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfiles { get; set; } = default!;



    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public Guid DocumentGuid { get; set; } = default!;


    /// <inheritdoc/>
    public UserInfoModel? CurrentUser { get; private set; }

    SessionOfDocumentDataModelDB SessionDocument = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;

        TResponseModel<UserInfoModel?> currentUser = await UsersProfiles.FindByIdAsync();
        CurrentUser = currentUser.Response;

        TResponseModel<SessionOfDocumentDataModelDB> rest = await FormsRepo.GetSessionDocumentData(DocumentGuid.ToString());

        IsBusyProgress = false;

        if (rest.Response is null)
            throw new Exception("rest.SessionDocument is null. error 5E20961A-3F1A-4409-9481-FA623F818918");

        SessionDocument = rest.Response;
        if (SessionDocument.DataSessionValues is not null && SessionDocument.DataSessionValues.Count != 0)
            SessionDocument.DataSessionValues.ForEach(x => { x.Owner ??= SessionDocument; x.OwnerId = SessionDocument.Id; });
    }
}