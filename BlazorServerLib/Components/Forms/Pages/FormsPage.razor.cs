using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Pages;

/// <inheritdoc/>
public partial class FormsPage : ComponentBase
{
    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfiles { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    protected UserInfoModel CurrentUser = default!;

    /// <inheritdoc/>
    protected EntryDescriptionModel? CurrentMainProject = null;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        TResponseModel<UserInfoModel?> restUser = await UsersProfiles.FindByIdAsync();
        if (restUser.Response is null)
            throw new Exception("Current user is null");

        if (!restUser.Success())
            SnackbarRepo.ShowMessagesResponse(restUser.Messages);
        CurrentUser = restUser.Response;

        TResponseModel<EntryDescriptionModel> restProject = await FormsRepo.GetCurrentMainProject(CurrentUser.UserId);
        CurrentMainProject = restProject.Response;
        if(!restProject.Success())
            SnackbarRepo.ShowMessagesResponse(restProject.Messages);
    }
}
