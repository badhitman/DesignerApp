using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// ElementDirectoryFieldSet
/// </summary>
public partial class ElementDirectoryFieldSetComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required int SelectedDirectoryId { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public SystemEntryModel ElementObject { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public Action<int> DeleteElementOfDirectoryHandler { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required DirectoryElementsListViewComponent ParentDirectoryElementsList { get; set; }

    bool IsEdit = false;
    async void EditDoneHandle()
    {
        IsEdit = false;
        await ParentDirectoryElementsList.ReloadElements(null, true);
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected async Task DeleteElementOfDirectory()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.DeleteElementFromDirectory(ElementObject.Id);
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        await FormsRepo.CheckAndNormalizeSortIndexForElementsOfDirectory(SelectedDirectoryId);
        IsBusyProgress = false;
        await ParentDirectoryElementsList.ReloadElements(null, true);
    }
}
