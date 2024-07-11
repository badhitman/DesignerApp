////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using BlazorWebLib.Components.Constructor.Shared.Manufacture;

namespace BlazorWebLib.Components.Constructor.Pages;

/// <inheritdoc/>
public partial class ConstructorPage : BlazorBusyComponentBaseModel
{
    [Inject]
    IConstructorService ConstructorRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfiles { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IManufactureService ManufactureRepo { get; set; } = default!;


    ManufactureComponent? manufacture_ref = default!;


    /// <inheritdoc/>
    public List<SystemNameEntryModel> SystemNamesManufacture = default!;

    /// <inheritdoc/>
    UserInfoModel CurrentUser = default!;

    /// <inheritdoc/>
    public MainProjectViewModel? MainProject { get; private set; }

    /// <summary>
    /// Проверка разрешения редактировать проект
    /// </summary>
    public bool CanEditProject { get; private set; }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<UserInfoModel?> currentUser = await UsersProfiles.FindByIdAsync();

        if (currentUser.Response is null)
            throw new Exception("Current user is null");

        IsBusyProgress = false;

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
        CanEditProject = false;
        IsBusyProgress = true;
        TResponseModel<MainProjectViewModel> currentMainProject = await ConstructorRepo.GetCurrentMainProject(CurrentUser.UserId);

        if (!currentMainProject.Success())
            SnackbarRepo.ShowMessagesResponse(currentMainProject.Messages);

        MainProject = currentMainProject.Response;
        CanEditProject = MainProject is not null && (!MainProject.IsDisabled || MainProject.OwnerUserId.Equals(CurrentUser.UserId) || CurrentUser.IsAdmin);
        IsBusyProgress = false;
        await GetSystemNames();

    }

    /// <inheritdoc/>
    public async Task GetSystemNames()
    {
        IsBusyProgress = true;
        if (MainProject is not null)
            SystemNamesManufacture = await ManufactureRepo.GetSystemNames(MainProject!.Id);
        IsBusyProgress = false;
        manufacture_ref?.StateHasChangedCall();
    }
}