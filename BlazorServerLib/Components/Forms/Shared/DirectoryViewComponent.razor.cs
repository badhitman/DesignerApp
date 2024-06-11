using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Directory view
/// </summary>
public partial class DirectoryViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    protected DirectoryNavComponent? _creator_ref;
    /// <inheritdoc/>
    protected IEnumerable<EntryModel>? directories_all;
    /// <inheritdoc/>
    protected DirectoryElementsListViewComponent? list_view_ref;
    /// <inheritdoc/>
    protected bool IsEditDirectory = false;

    int _selected_dir_id;
    int SelectedDirectoryId
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

    string? DirectoryName { get; set; }
    string? ElementDirectoryName { get; set; }

    /// <inheritdoc/>
    protected async void AddElementIntoDirectory()
    {
        if (string.IsNullOrWhiteSpace(ElementDirectoryName))
        {
            SnackbarRepo.Add("Укажите имя элемента", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        IsBusyProgress = true;
        CreateObjectOfIntKeyResponseModel rest = await FormsRepo.CreateElementForDirectory(ElementDirectoryName, SelectedDirectoryId);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        ElementDirectoryName = string.Empty;
        if (list_view_ref is not null)
            await InvokeAsync(async () => await list_view_ref.ReloadElements(_selected_dir_id, true));
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected void DeleteElementOfDirectory(int element_id)
    {
        IsBusyProgress = true;
        InvokeAsync(async () =>
        {
            ResponseBaseModel rest = await FormsRepo.DeleteElementFromDirectory(element_id);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            if (list_view_ref is not null)
                await list_view_ref.ReloadElements(_selected_dir_id, true);
            StateHasChanged();
        });
    }

    /// <inheritdoc/>
    protected void RenameSelectedDirectoryAction(bool state)
    {
        IsEditDirectory = state;
        DirectoryName = state ? directories_all?.FirstOrDefault(x => x.Id == SelectedDirectoryId)?.Name : "";
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected void SaveRenameDirectoryAction()
    {
        if (string.IsNullOrWhiteSpace(DirectoryName))
        {
            SnackbarRepo.Add($"Имя справочника не может быть пустым", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        IsBusyProgress = true;
        _ = InvokeAsync(async () =>
        {
            CreateObjectOfIntKeyResponseModel rest = await FormsRepo.UpdateOrCreateDirectory(new EntryModel() { Id = SelectedDirectoryId, Name = DirectoryName });
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            _creator_ref?.SetDirectoryNavState(DirectoryNavStatesEnum.None);
            IsEditDirectory = false;
            await ReloadDirectories();
        });
    }

    /// <inheritdoc/>
    protected void DeleteSelectedDirectoryAction()
    {
        IsBusyProgress = true;
        _ = InvokeAsync(async () =>
        {
            ResponseBaseModel rest = await FormsRepo.DeleteDirectory(SelectedDirectoryId);

            SnackbarRepo.ShowMessagesResponse(rest.Messages);

            await ReloadDirectories();
            StateHasChanged();
        });
    }

    /// <inheritdoc/>
    protected void CreateDirectoryAction(string name)
    {
        IsBusyProgress = true;
        _ = InvokeAsync(async () =>
        {
            CreateObjectOfIntKeyResponseModel rest = await FormsRepo.UpdateOrCreateDirectory(new EntryModel() { Name = name });
            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            SelectedDirectoryId = rest.Id;
            await ReloadDirectories();
            StateHasChanged();
        });
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync() => await ReloadDirectories();

    async Task ReloadDirectories()
    {
        ElementDirectoryName = string.Empty;
        IsBusyProgress = true;
        EntriesResponseModel rest = await FormsRepo.GetDirectories();
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        directories_all = rest.Entries;

        if (directories_all?.Any() != true)
            SelectedDirectoryId = -1;

        if (directories_all?.Any(x => x.Id == SelectedDirectoryId) != true)
            SelectedDirectoryId = directories_all?.FirstOrDefault()?.Id ?? 0;

        StateHasChanged();
    }
}