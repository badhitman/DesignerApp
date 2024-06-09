using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class DirectoryViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<DirectoryViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected IJSRuntime _js_runtime { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    protected DirectoryNavComponent? _creator_ref;
    protected IEnumerable<EntryModel>? directories_all;
    protected DirectoryElementsListViewComponent? list_view_ref;
    protected bool IsEditDirectory = false;

    int _selected_dir_id;
    int selected_dir_id
    {
        get => _selected_dir_id;
        set
        {
            _selected_dir_id = value;

            if (list_view_ref is not null)
                InvokeAsync(async () => await list_view_ref.ReloadElements(_selected_dir_id, true));
            _creator_ref?.StateHasChangedAction(_selected_dir_id > 0);
        }
    }

    string? directory_name { get; set; }
    string? element_directory_name { get; set; }

    protected async void AddElementIntoDirectory()
    {
        if (string.IsNullOrWhiteSpace(element_directory_name))
        {
            _snackbar.Add("Укажите имя элемента", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        IsBusyProgress = true;
        CreateObjectOfIntKeyResponseModel rest = await _forms.CreateElementOfDirectory(new EntryModel() { Id = selected_dir_id, Name = element_directory_name });
        IsBusyProgress = false;
        _snackbar.ShowMessagesResponse(rest.Messages);
        element_directory_name = string.Empty;
        if (list_view_ref is not null)
            await InvokeAsync(async () => await list_view_ref.ReloadElements(_selected_dir_id, true));
        StateHasChanged();
    }

    protected void DeleteElementOfDirectory(int element_id)
    {
        IsBusyProgress = true;
        InvokeAsync(async () =>
        {
            ResponseBaseModel rest = await _forms.DeleteElementOfDirectory(element_id);
            IsBusyProgress = false;

            _snackbar.ShowMessagesResponse(rest.Messages);
            if (list_view_ref is not null)
                await list_view_ref.ReloadElements(_selected_dir_id, true);
            StateHasChanged();
        });
    }

    protected void RenameSelectedDirectoryAction(bool state)
    {
        IsEditDirectory = state;
        directory_name = state ? directories_all?.FirstOrDefault(x => x.Id == selected_dir_id)?.Name : "";
        StateHasChanged();
    }

    protected void SaveRenameDirectoryAction()
    {
        if (string.IsNullOrWhiteSpace(directory_name))
        {
            _snackbar.Add($"Имя справочника не может быть пустым", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        IsBusyProgress = true;
        _ = InvokeAsync(async () =>
        {
            CreateObjectOfIntKeyResponseModel rest = await _forms.UpdateOrCreateDirectory(new EntryModel() { Id = selected_dir_id, Name = directory_name });
            IsBusyProgress = false;
            _snackbar.ShowMessagesResponse(rest.Messages);
            _creator_ref?.SetDirectoryNavState(DirectoryNavStatesEnum.None);
            IsEditDirectory = false;
            await ReloadDirectories();
        });
    }

    protected void DeleteSelectedDirectoryAction()
    {
        IsBusyProgress = true;
        _ = InvokeAsync(async () =>
        {
            ResponseBaseModel rest = await _forms.DeleteDirectory(selected_dir_id);

            _snackbar.ShowMessagesResponse(rest.Messages);

            await ReloadDirectories();
            StateHasChanged();
        });
    }

    protected void CreateDirectoryAction(string name)
    {
        IsBusyProgress = true;
        _ = InvokeAsync(async () =>
        {
            CreateObjectOfIntKeyResponseModel rest = await _forms.UpdateOrCreateDirectory(new EntryModel() { Name = name });
            _snackbar.ShowMessagesResponse(rest.Messages);
            selected_dir_id = rest.Id;
            await ReloadDirectories();
            StateHasChanged();
        });
    }

    protected override async Task OnInitializedAsync() => await ReloadDirectories();

    async Task ReloadDirectories()
    {
        element_directory_name = string.Empty;
        IsBusyProgress = true;
        EntriesResponseModel rest = await _forms.GetDirectories();
        IsBusyProgress = false;
        _snackbar.ShowMessagesResponse(rest.Messages);

        directories_all = rest.Entries;

        if (directories_all?.Any() != true)
            selected_dir_id = -1;

        if (directories_all?.Any(x => x.Id == selected_dir_id) != true)
            selected_dir_id = directories_all?.FirstOrDefault()?.Id ?? 0;

        StateHasChanged();
    }
}