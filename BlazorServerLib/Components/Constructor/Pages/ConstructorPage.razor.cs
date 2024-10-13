////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Shared.Manufacture;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Pages;

/// <inheritdoc/>
public partial class ConstructorPage : BlazorBusyComponentBaseModel
{
    [Inject]
    AuthenticationStateProvider AuthRepo { get; set; } = default!;

    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IManufactureService ManufactureRepo { get; set; } = default!;


    ManufactureComponent? manufacture_ref = default!;


    /// <inheritdoc/>
    public List<SystemNameEntryModel> SystemNamesManufacture = default!;

    /// <inheritdoc/>
    UserInfoMainModel CurrentUser = default!;

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
        AuthenticationState state = await AuthRepo.GetAuthenticationStateAsync();
        CurrentUser = state.User.ReadCurrentUserInfo() ?? throw new Exception();

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