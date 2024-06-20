using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using SharedLib;
using BlazorLib;
using MudBlazor;
using System.Text.RegularExpressions;

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
    protected SystemEntryModel[] directories_all = default!;
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
    string? DirectorySystemName { get; set; }
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
        if (IsEditDirectory)
        {
            SystemEntryModel current_directory = directories_all.First(x => x.Id == SelectedDirectoryId);
            DirectoryName = current_directory.Name;
            DirectorySystemName = current_directory.SystemName;
        }

        StateHasChanged();
    }

    /// <inheritdoc/>
    protected async void SaveRenameDirectoryAction()
    {
        if (string.IsNullOrWhiteSpace(DirectoryName))
        {
            SnackbarRepo.Add($"Название справочника не может быть пустым", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (string.IsNullOrWhiteSpace(DirectorySystemName) || !Regex.IsMatch(DirectorySystemName, GlobalStaticConstants.NAME_SPACE_TEMPLATE))
        {
            SnackbarRepo.Add($"Системное имя не корректное. Оно может содержать латинские буквы и цифры. Первым символом должна идти буква", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (ParentFormsPage.MainProject is null)
        {
            SnackbarRepo.Add("Не выбран текущий/основной проект", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        IsBusyProgress = true;
        TResponseStrictModel<int> rest = await FormsRepo.UpdateOrCreateDirectory(new EntryConstructedModel() { Id = SelectedDirectoryId, Name = DirectoryName, SystemName = DirectorySystemName, ProjectId = ParentFormsPage.MainProject.Id });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        _creator_ref?.SetDirectoryNavState(DirectoryNavStatesEnum.None);
        IsEditDirectory = false;
        await ReloadDirectories();
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
        if (ParentFormsPage.MainProject is null)
        {
            SnackbarRepo.Add("Не выбран текущий/основной проект", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        IsBusyProgress = true;
        TResponseStrictModel<int> rest = await FormsRepo.UpdateOrCreateDirectory(new EntryConstructedModel() { Name = dir.Name, SystemName = dir.SystemName, ProjectId = ParentFormsPage.MainProject.Id });
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
        TResponseStrictModel<SystemEntryModel[]> rest = await FormsRepo.GetDirectories(ParentFormsPage.MainProject.Id);
        IsBusyProgress = false;

        directories_all = rest.Response;

        if (directories_all.Length == 0)
            SelectedDirectoryId = -1;
        else if (directories_all.Any(x => x.Id == SelectedDirectoryId) != true)
        {
            if (SelectedDirectoryId != 0)
                SnackbarRepo.Add($"Выбранный справочник #{SelectedDirectoryId} больше не существует!", Severity.Warning, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            SelectedDirectoryId = directories_all.FirstOrDefault()?.Id ?? 0;
        }

        StateHasChanged();
    }
}