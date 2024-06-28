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
public partial class QuestionnaireClientViewPage : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfiles { get; set; } = default!;



    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public Guid QuestionnaireGuid { get; set; } = default!;


    /// <inheritdoc/>
    public UserInfoModel? CurrentUser { get; private set; }

    SessionOfDocumentDataModelDB SessionQuestionnaire = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;

        TResponseModel<UserInfoModel?> currentUser = await UsersProfiles.FindByIdAsync();
        CurrentUser = currentUser.Response;

        TResponseModel<SessionOfDocumentDataModelDB> rest = await FormsRepo.GetSessionDocumentData(QuestionnaireGuid.ToString());

        IsBusyProgress = false;

        if (rest.Response is null)
            throw new Exception("rest.SessionQuestionnaire is null. error 5E20961A-3F1A-4409-9481-FA623F818918");

        SessionQuestionnaire = rest.Response;
        if (SessionQuestionnaire.DataSessionValues is not null && SessionQuestionnaire.DataSessionValues.Count != 0)
            SessionQuestionnaire.DataSessionValues.ForEach(x => { x.Owner ??= SessionQuestionnaire; x.OwnerId = SessionQuestionnaire.Id; });
    }
}