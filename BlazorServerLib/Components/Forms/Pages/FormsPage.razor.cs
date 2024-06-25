using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Pages;

/// <inheritdoc/>
public partial class FormsPage : BlazorBusyComponentBaseModel
{
    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfiles { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    public UserInfoModel CurrentUser { get; private set; } = default!;

    /// <inheritdoc/>
    public MainProjectViewModel? MainProject { get; set; }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        TResponseModel<UserInfoModel?> currentUser = await UsersProfiles.FindByIdAsync();
        if (currentUser.Response is null)
            throw new Exception("Current user is null");

        CurrentUser = currentUser.Response;
        if (!currentUser.Success())
            SnackbarRepo.ShowMessagesResponse(currentUser.Messages);

        await ReadCurrentMainProject();
    }

    /// <summary>
    /// Прочитать данные о текущем/основном проекте
    /// </summary>
    public async Task ReadCurrentMainProject()
    {
        TResponseModel<MainProjectViewModel> currentMainProject = await FormsRepo.GetCurrentMainProject(CurrentUser.UserId);
        if (!currentMainProject.Success())
            SnackbarRepo.ShowMessagesResponse(currentMainProject.Messages);

        MainProject = currentMainProject.Response;
    }
}