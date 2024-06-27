////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Pages;

/// <inheritdoc/>
public partial class QuestionnaireClientViewIntPage : BlazorBusyComponentBaseModel
{
    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfiles { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public int QuestionnaireId { get; set; }



    /// <inheritdoc/>
    public UserInfoModel? CurrentUser { get; private set; }

    SessionOfDocumentDataModelDB SessionQuestionnaire = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;        
        TResponseModel<UserInfoModel?> currentUser = await UsersProfiles.FindByIdAsync();
        CurrentUser = currentUser.Response;

        TResponseModel<SessionOfDocumentDataModelDB> rest = await FormsRepo.GetSessionQuestionnaire(QuestionnaireId);
        
        IsBusyProgress = false;

        if (rest.Response is null)
            throw new Exception("rest.Content.SessionQuestionnaire is null. error 09DFC142-55DA-4616-AA14-EA1B810E9A7E");

        SessionQuestionnaire = rest.Response;
        if (SessionQuestionnaire.DataSessionValues is not null && SessionQuestionnaire.DataSessionValues.Count != 0)
            SessionQuestionnaire.DataSessionValues.ForEach(x => { x.Owner ??= SessionQuestionnaire; x.OwnerId = SessionQuestionnaire.Id; });
    }
}