////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Directory Navigation
/// </summary>
public partial class DirectoryNavComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }

    /// <summary>
    /// Событие изменения выбранного справочника/списка
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<int> SelectedDirectoryChangeHandler { get; set; }

    SystemEntryModel[] allDirectories = default!;

    /// <summary>
    /// Выбранный справочник/список
    /// </summary>
    public int SelectedDirectoryId
    {
        get => _selected_dir_id;
        private set
        {
            if (_selected_dir_id != value)
                SelectedDirectoryChangeHandler(value);

            _selected_dir_id = value;
        }
    }
    int _selected_dir_id;

    SystemEntryModel directoryObject = default!;

    static readonly DirectoryNavStatesEnum[] ModesForHideSelector = [DirectoryNavStatesEnum.Create, DirectoryNavStatesEnum.Rename];

    /// <summary>
    /// Directory navigation state
    /// </summary>
    protected DirectoryNavStatesEnum DirectoryNavState = DirectoryNavStatesEnum.None;

    /// <summary>
    /// Текст кнопки создания справочника
    /// </summary>
    protected string GetTitleForButtonCreate
    {
        get
        {
            if (string.IsNullOrWhiteSpace(directoryObject.Name) || string.IsNullOrWhiteSpace(directoryObject.SystemName))
                return "Введите название и системное имя";

            return "Создать";
        }
    }

    void InitRenameDirectory()
    {
        directoryObject = allDirectories.First(x => x.Id == SelectedDirectoryId);
        DirectoryNavState = DirectoryNavStatesEnum.Rename;
    }

    /// <inheritdoc/>
    protected async Task DeleteSelectedDirectory()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.DeleteDirectory(SelectedDirectoryId);
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        await ReloadDirectories();
        SelectedDirectoryChangeHandler(SelectedDirectoryId);
    }

    /// <summary>
    /// Перезагрузить селектор справочников/списков
    /// </summary>
    public async Task ReloadDirectories(bool stateHasChanged = false)
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран основной/используемый проект");

        ResetNavForm();

        IsBusyProgress = true;
        TResponseStrictModel<SystemEntryModel[]> rest = await FormsRepo.GetDirectories(ParentFormsPage.MainProject.Id);
        IsBusyProgress = false;

        allDirectories = rest.Response;

        if (allDirectories.Length == 0)
            SelectedDirectoryId = -1;
        else if (allDirectories.Any(x => x.Id == SelectedDirectoryId) != true)
            SelectedDirectoryId = allDirectories.FirstOrDefault()?.Id ?? 0;

        SelectedDirectoryChangeHandler(SelectedDirectoryId);

        if (stateHasChanged)
            StateHasChanged();
    }

    /// <inheritdoc/>
    protected async Task CreateDirectory()
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран текущий/основной проект");

        IsBusyProgress = true;
        TResponseStrictModel<int> rest = await FormsRepo.UpdateOrCreateDirectory(new EntryConstructedModel() { Name = directoryObject.Name, SystemName = directoryObject.SystemName, ProjectId = ParentFormsPage.MainProject.Id });
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Success())
        {
            ResetNavForm();
            await ReloadDirectories();
            SelectedDirectoryId = rest.Response;
        }
    }

    /// <inheritdoc/>
    protected async Task SaveRenameDirectory()
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран текущий/основной проект");

        IsBusyProgress = true;
        TResponseStrictModel<int> rest = await FormsRepo.UpdateOrCreateDirectory(EntryConstructedModel.Build(directoryObject, ParentFormsPage.MainProject.Id));
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        await ReloadDirectories();
    }

    void ResetNavForm(bool stateHasChanged = false)
    {
        directoryObject = SystemEntryModel.BuildEmpty();
        DirectoryNavState = DirectoryNavStatesEnum.None;

        if (stateHasChanged)
            StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReloadDirectories();
    }
}