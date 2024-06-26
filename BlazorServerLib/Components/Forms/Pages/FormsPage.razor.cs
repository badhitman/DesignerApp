////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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

    /// <summary>
    /// Проверка разрешения редактировать проект
    /// </summary>
    public bool CanEditProject => MainProject is not null && (!MainProject.IsDisabled || MainProject.OwnerUserId.Equals(CurrentUser.UserId) || CurrentUser.IsAdmin);

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<UserInfoModel?> currentUser = await UsersProfiles.FindByIdAsync();
        IsBusyProgress = false;
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
        IsBusyProgress = true;
        TResponseModel<MainProjectViewModel> currentMainProject = await FormsRepo.GetCurrentMainProject(CurrentUser.UserId);
        IsBusyProgress = false;
        if (!currentMainProject.Success())
            SnackbarRepo.ShowMessagesResponse(currentMainProject.Messages);

        MainProject = currentMainProject.Response;
    }
}