using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class DirectoryElementsListViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<DirectoryElementsListViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected IJSRuntime _js_runtime { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    [Parameter, EditorRequired]
    public int SelectedDirectoryId { get; set; }

    protected IEnumerable<EntryModel>? EntriesElements;

    public async Task ReloadElements(int? selected_directory_id = null, bool state_has_change = false)
    {
        if (selected_directory_id is not null)
            SelectedDirectoryId = selected_directory_id.Value;

        if (SelectedDirectoryId <= 0)
        {
            _snackbar.Add($"Идентификатор справочника не может быть меньше 0 ({SelectedDirectoryId})", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        IsBusyProgress = true;

        EntriesResponseModel rest = await _forms.GetElementsDirectories(SelectedDirectoryId);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        EntriesElements = rest.Entries ?? Enumerable.Empty<EntryModel>();

        if (state_has_change)
            StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        if (SelectedDirectoryId > 0)
            await ReloadElements();
    }
}