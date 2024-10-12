////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using static SharedLib.GlobalStaticConstants;

namespace BlazorWebLib.Components.Constructor.Shared.DirectoriesCatalog;

/// <summary>
/// Directory Navigation
/// </summary>
public partial class DirectoryNavComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IConstructorService ConstructorRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }

    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }

    /// <summary>
    /// Событие изменения выбранного справочника/списка
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<int> SelectedDirectoryChangeHandler { get; set; }


    /// <summary>
    /// Current Template InputRichText ref
    /// </summary>
    protected InputRichTextComponent? _currentTemplateInputRichText_ref;
    string images_upload_url = default!;
    Dictionary<string, object> editorConf = default!;

    EntryModel[] allDirectories = default!;

    EntryDescriptionModel? selectedDirectory;

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
            if (_selected_dir_id > 0)
                InvokeAsync(async () =>
                {
                    selectedDirectory = await ConstructorRepo.GetDirectory(value);
                    Description = selectedDirectory.Description;
                });
        }
    }
    int _selected_dir_id;

    EntryModel directoryObject = default!;
    string? Description { get; set; }

    static readonly DirectoryNavStatesEnum[] ModesForHideSelector =
        [
        DirectoryNavStatesEnum.Create,
        DirectoryNavStatesEnum.Rename,
        DirectoryNavStatesEnum.Delete
        ];

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
            if (string.IsNullOrWhiteSpace(directoryObject.Name))
                return "Введите название";

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
        ResponseBaseModel rest = await ConstructorRepo.DeleteDirectory(SelectedDirectoryId);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }

        await ReloadDirectories();
        SelectedDirectoryChangeHandler(SelectedDirectoryId);
    }

    /// <inheritdoc/>
    protected async Task CreateDirectory()
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран текущий/основной проект");

        IsBusyProgress = true;
        TResponseStrictModel<int> rest = await ConstructorRepo.UpdateOrCreateDirectory(new EntryConstructedModel() { Name = directoryObject.Name, ProjectId = ParentFormsPage.MainProject.Id, Description = Description });
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Success())
        {
            ResetNavForm();
            await ReloadDirectories();
            SelectedDirectoryId = rest.Response;
        }
        else
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }
    }

    async Task CancelCreatingDirectory()
    {
        ResetNavForm();
        selectedDirectory = await ConstructorRepo.GetDirectory(_selected_dir_id);
        Description = selectedDirectory.Description;
    }

    /// <inheritdoc/>
    protected async Task SaveRenameDirectory()
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран текущий/основной проект");

        IsBusyProgress = true;
        TResponseStrictModel<int> rest = await ConstructorRepo.UpdateOrCreateDirectory(EntryConstructedModel.Build(directoryObject, ParentFormsPage.MainProject.Id, Description));
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }

        await ReloadDirectories();
    }

    void ResetNavForm(bool stateHasChanged = false)
    {
        directoryObject = EntryModel.BuildEmpty();
        Description = null;
        DirectoryNavState = DirectoryNavStatesEnum.None;

        if (stateHasChanged)
            StateHasChanged();
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
        TResponseStrictModel<EntryModel[]> rest = await ConstructorRepo.GetDirectories(ParentFormsPage.MainProject.Id);
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
    protected override async Task OnInitializedAsync()
    {
        images_upload_url = $"/TinyMCEditor/UploadImage/{Routes.CONSTRUCTOR_CONTROLLER_NAME}/{Routes.DIRECTORY_CONTROLLER_NAME}?{nameof(StorageMetadataModel.PrefixPropertyName)}={Routes.DEFAULT_CONTROLLER_NAME}&{nameof(StorageMetadataModel.OwnerPrimaryKey)}={SelectedDirectoryId}";
        editorConf = TinyMCEditorConf(images_upload_url);
        await ReloadDirectories();
    }
}