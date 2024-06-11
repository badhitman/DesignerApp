using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Directory elements-list view
/// </summary>
public partial class DirectoryElementsListViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required int SelectedDirectoryId { get; set; }

    /// <inheritdoc/>
    protected IEnumerable<EntryModel>? EntriesElements;

    /// <inheritdoc/>
    public async Task ReloadElements(int? selected_directory_id = null, bool state_has_change = false)
    {
        if (selected_directory_id is not null)
            SelectedDirectoryId = selected_directory_id.Value;

        if (SelectedDirectoryId <= 0)
        {
            SnackbarRepo.Add($"Идентификатор справочника не может быть меньше 0 ({SelectedDirectoryId})", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        IsBusyProgress = true;

        EntriesResponseModel rest = await FormsRepo.GetElementsOfDirectory(SelectedDirectoryId);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        EntriesElements = rest.Entries ?? [];

        if (state_has_change)
            StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (SelectedDirectoryId > 0)
            await ReloadElements();
    }
}