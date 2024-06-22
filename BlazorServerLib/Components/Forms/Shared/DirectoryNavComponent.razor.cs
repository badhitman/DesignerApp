using BlazorLib;
using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
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

    /// <inheritdoc/>
    protected bool IsEditDirectoryMode = false;

    SystemEntryModel EditDirectoryObject = default!;
    SystemOwnedNameModel elementForDict = default!;

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
            if (elementForDict is null)
                throw new ArgumentNullException(nameof(elementForDict), "Элемент справочника/списка не инициализирован");

            if (string.IsNullOrWhiteSpace(elementForDict.Name) || string.IsNullOrWhiteSpace(elementForDict.SystemName))
                return "Введите название";

            return "Создать";
        }
    }


    /// <inheritdoc/>
    protected async void AddElementIntoDirectoryAction()
    {
        if (SelectedDirectoryId < 1)
            throw new Exception("Не выбран справочник/список");

        IsBusyProgress = true;
        TResponseStrictModel<int> rest = await FormsRepo.CreateElementForDirectory(elementForDict, SelectedDirectoryId);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
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

        if (stateHasChanged)
            StateHasChanged();
    }

    void TryCreateDirectory()
    {
        //if (string.IsNullOrWhiteSpace(NameNewDict) || string.IsNullOrWhiteSpace(SystemCodeNewDict))
        //{
        //    SnackbarRepo.Add("Системное имя и название обязательны", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
        //    return;
        //}

        //CreateDirectoryHandler((Name: NameNewDict, SystemName: SystemCodeNewDict));
        //ResetNavForm();
    }

    /// <inheritdoc/>
    protected async void SaveRenameDirectoryAction()
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран текущий/основной проект");

        IsBusyProgress = true;
        TResponseStrictModel<int> rest = await FormsRepo.UpdateOrCreateDirectory(EntryConstructedModel.Build(EditDirectoryObject, ParentFormsPage.MainProject.Id));
        IsBusyProgress = false;
        //SnackbarRepo.ShowMessagesResponse(rest.Messages);
        //_creator_ref?.SetDirectoryNavState(DirectoryNavStatesEnum.None);
        //IsEditDirectory = false;
        //EditDirectoryObject = null;
        //await ReloadDirectories();
    }

    void ResetNavForm(bool stateHasChanged = false)
    {
        EditDirectoryObject = SystemEntryModel.BuildEmpty();
        elementForDict = SystemOwnedNameModel.BuildEmpty(SelectedDirectoryId);
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