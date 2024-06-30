////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Pages;

/// <inheritdoc/>
public partial class DocumentClientViewIntPage : BlazorBusyComponentBaseModel
{
    [Inject]
    IConstructorService ConstructorRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfiles { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public int DocumentId { get; set; }



    /// <inheritdoc/>
    public UserInfoModel? CurrentUser { get; private set; }

    SessionOfDocumentDataModelDB SessionDocument = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;        
        TResponseModel<UserInfoModel?> currentUser = await UsersProfiles.FindByIdAsync();
        CurrentUser = currentUser.Response;

        TResponseModel<SessionOfDocumentDataModelDB> rest = await ConstructorRepo.GetSessionDocument(DocumentId);
        
        IsBusyProgress = false;

        if (rest.Response is null)
            throw new Exception("rest.Content.SessionDocument is null. error 09DFC142-55DA-4616-AA14-EA1B810E9A7E");

        SessionDocument = rest.Response;
        if (SessionDocument.DataSessionValues is not null && SessionDocument.DataSessionValues.Count != 0)
            SessionDocument.DataSessionValues.ForEach(x => { x.Owner ??= SessionDocument; x.OwnerId = SessionDocument.Id; });
    }
}