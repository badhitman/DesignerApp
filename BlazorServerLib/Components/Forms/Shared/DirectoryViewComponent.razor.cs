////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Directory view
/// </summary>
public partial class DirectoryViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    protected DirectoryElementsListViewComponent elementsListOfDirectoryView_ref = default!;

    /// <inheritdoc/>
    protected DirectoryNavComponent directoryNav_ref = default!;

    SystemOwnedNameModel createNewElementForDict = SystemOwnedNameModel.BuildEmpty(0);

    /// <inheritdoc/>
    protected async void AddElementIntoDirectory()
    {
        if (createNewElementForDict.OwnerId < 1)
            throw new Exception("Не выбран справочник/список");

        directoryNav_ref.IsBusyProgress = true;
        directoryNav_ref.StateHasChangedCall();

        IsBusyProgress = true;
        TResponseStrictModel<int> rest = await FormsRepo.CreateElementForDirectory(createNewElementForDict);
        IsBusyProgress = false;

        directoryNav_ref.IsBusyProgress = false;
        directoryNav_ref.StateHasChangedCall();

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        createNewElementForDict = SystemOwnedNameModel.BuildEmpty(createNewElementForDict.OwnerId);
        if (rest.Success())
            await elementsListOfDirectoryView_ref.ReloadElements(directoryNav_ref.SelectedDirectoryId, true);
    }

    async void SelectedDirectoryChangeAction(int selectedDirectoryId)
    {
        createNewElementForDict = SystemOwnedNameModel.BuildEmpty(selectedDirectoryId);
        await elementsListOfDirectoryView_ref.ReloadElements(selectedDirectoryId, true);
        StateHasChanged();
    }
}