using BlazorLib;
using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Directory view
/// </summary>
public partial class DirectoryViewComponent : BlazorBusyComponentBaseModel
{
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
        TResponseStrictModel<int> rest = await FormsRepo.CreateElementForDirectory(ElementDirectoryName, SelectedDirectoryId);
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
            TResponseStrictModel<int> rest = await FormsRepo.UpdateOrCreateDirectory(new SystemEntryModel() { Id = SelectedDirectoryId, Name = DirectoryName, SystemName = "" });
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
    protected async void CreateDirectoryAction((string Name, string SystemName) dir)
    {
        IsBusyProgress = true;
        TResponseStrictModel<int> rest = await FormsRepo.UpdateOrCreateDirectory(new SystemEntryModel() { Name = dir.Name, SystemName = dir.SystemName });
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        SelectedDirectoryId = rest.Response;
        await ReloadDirectories();
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync() => await ReloadDirectories();

    async Task ReloadDirectories()
    {
        if (ParentFormsPage.MainProject is null)
        {
            SnackbarRepo.Add("Не выбран основной/используемый проект", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        ElementDirectoryName = string.Empty;
        IsBusyProgress = true;
        TResponseModel<EntryModel[]> rest = await FormsRepo.GetDirectories(ParentFormsPage.MainProject.Id);
        IsBusyProgress = false;

        directories_all = rest.Response;

        if (directories_all?.Any() != true)
            SelectedDirectoryId = -1;
        else if (directories_all.Any(x => x.Id == SelectedDirectoryId) != true)
        {
            SnackbarRepo.Add($"Выбранный справочник #{SelectedDirectoryId} больше не существует!", Severity.Warning, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            SelectedDirectoryId = directories_all.FirstOrDefault()?.Id ?? 0;
        }

        StateHasChanged();
    }
}