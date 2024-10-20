////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;

namespace BlazorWebLib.Components.Constructor.Shared.DirectoriesCatalog;

/// <summary>
/// Directory view
/// </summary>
public partial class DirectoryViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;


    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }


    UserInfoMainModel CurrentUser = default!;

    /// <inheritdoc/>
    protected ElementsOfDirectoryListViewComponent elementsListOfDirectoryView_ref = default!;

    /// <inheritdoc/>
    protected DirectoryNavComponent? directoryNav_ref;

    OwnedNameModel createNewElementForDict = OwnedNameModel.BuildEmpty(0);

    /// <inheritdoc/>
    protected async void AddElementIntoDirectory()
    {
        if (createNewElementForDict.OwnerId < 1)
            throw new Exception("Не выбран справочник/список");

        ValidateReportModel validate_obj = GlobalTools.ValidateObject(createNewElementForDict);
        if (!validate_obj.IsValid)
        {
            SnackbarRepo.Error(validate_obj.ValidationResults);
            return;
        }

        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<int> rest = await ConstructorRepo.CreateElementForDirectory(new() { Payload = createNewElementForDict, SenderActionUserId = CurrentUser.UserId }); // (new() { Payload = createNewElementForDict, SenderActionUserId = user.UserId });
        createNewElementForDict = OwnedNameModel.BuildEmpty(createNewElementForDict.OwnerId);
        IsBusyProgress = false;
        StateHasChanged();
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (directoryNav_ref is not null)
        {
            directoryNav_ref.IsBusyProgress = true;
            directoryNav_ref.StateHasChangedCall();
        }

        if (rest.Success())
            await elementsListOfDirectoryView_ref.ReloadElements(directoryNav_ref?.SelectedDirectoryId, true);
        else
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }

        if (directoryNav_ref is not null)
        {
            directoryNav_ref.IsBusyProgress = false;
            directoryNav_ref.StateHasChangedCall();
        }
    }

    async void SelectedDirectoryChangeAction(int selectedDirectoryId)
    {
        createNewElementForDict = OwnedNameModel.BuildEmpty(selectedDirectoryId);
        await elementsListOfDirectoryView_ref.ReloadElements(selectedDirectoryId, true);
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        CurrentUser = state.User.ReadCurrentUserInfo() ?? throw new Exception();

        await base.OnInitializedAsync();
    }
}