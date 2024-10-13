////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using static MudBlazor.CategoryTypes;
using static SharedLib.GlobalStaticConstants;

namespace BlazorWebLib.Components.Constructor.Shared.DirectoriesCatalog;

/// <summary>
/// ElementDirectoryFieldSet
/// </summary>
public partial class ElementDirectoryFieldSetComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required int SelectedDirectoryId { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public EntryModel ElementObject { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public Action<int> DeleteElementOfDirectoryHandler { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ElementsOfDirectoryListViewComponent ParentDirectoryElementsList { get; set; }

    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }


    EntryDescriptionModel? ElementObjectOrign;
    EntryDescriptionModel? ElementObjectEdit;

    string images_upload_url = default!;
    Dictionary<string, object> editorConf = default!;

    /// <inheritdoc/>
    protected bool IsEdited => ElementObjectOrign is not null && !ElementObjectOrign.Equals(ElementObjectEdit);

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        images_upload_url = $"/TinyMCEditor/UploadImage/{Routes.CONSTRUCTOR_CONTROLLER_NAME}/{Routes.DIRECTORY_CONTROLLER_NAME}?{nameof(StorageMetadataModel.PrefixPropertyName)}={Routes.SET_ACTION_NAME}&{nameof(StorageMetadataModel.OwnerPrimaryKey)}={SelectedDirectoryId}";
        editorConf = TinyMCEditorConf(images_upload_url);
        await base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    protected async Task UpdateElementOfDirectory()
    {
        ArgumentNullException.ThrowIfNull(ElementObjectEdit);

        IsBusyProgress = true;
        await Task.Delay(1);
        ResponseBaseModel rest = await ConstructorRepo.UpdateElementOfDirectory(new() { Payload = ElementObjectEdit, SenderActionUserId = CurrentUser.UserId });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        ElementObjectOrign = GlobalTools.CreateDeepCopy(ElementObjectEdit);

        IsEdit = false;
        await ParentDirectoryElementsList.ReloadElements(null, true);
        StateHasChanged();

        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }
    }


    bool IsEdit = false;
    async Task EditToggle()
    {
        IsEdit = !IsEdit;
        if (!IsEdit)
        {
            ElementObjectOrign = null;
            ElementObjectEdit = null;
            return;
        }

        IsBusyProgress = true;
        TResponseModel<EntryDescriptionModel> res = await ConstructorRepo.GetElementOfDirectory(ElementObject.Id);
        ElementObjectOrign = res.Response ?? throw new Exception();
        ElementObjectEdit = GlobalTools.CreateDeepCopy(ElementObjectOrign);
        IsBusyProgress = false;
    }

    void RsetEdit()
    {
        ElementObjectEdit = GlobalTools.CreateDeepCopy(ElementObjectOrign);
    }

    /// <summary>
    /// Находится ли элемент в самом верхнем (крайнем) положении
    /// </summary>
    bool IsMostUp
    {
        get
        {
            if (ParentDirectoryElementsList.EntriesElements is null)
                throw new Exception("Элементы справочника IsNull");

            return ParentDirectoryElementsList
                .EntriesElements
                .FindIndex(x => x.Id == ElementObject.Id) == 0;
        }
    }
    /// <summary>
    /// Находится ли элемент в самом нижнем (крайнем) положении
    /// </summary>
    bool IsMostDown
    {
        get
        {
            if (ParentDirectoryElementsList.EntriesElements is null)
                throw new Exception("Элементы справочника IsNull");

            return ParentDirectoryElementsList
                .EntriesElements
                .FindIndex(x => x.Id == ElementObject.Id) == ParentDirectoryElementsList.EntriesElements.Count - 1;
        }
    }

    async Task MoveUp()
    {
        IsEdit = false;
        IsBusyProgress = true;
        await Task.Delay(1);
        ResponseBaseModel rest = await ConstructorRepo.UpMoveElementOfDirectory(new() { Payload = ElementObject.Id, SenderActionUserId = CurrentUser.UserId });
        IsBusyProgress = false;
        if (!rest.Success())
        {
            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }
        await ParentDirectoryElementsList.ReloadElements(null, true);
    }

    async Task MoveDown()
    {
        IsEdit = false;
        IsBusyProgress = true;
        await Task.Delay(1);
        ResponseBaseModel rest = await ConstructorRepo.DownMoveElementOfDirectory(new() { Payload = ElementObject.Id, SenderActionUserId = CurrentUser.UserId });
        IsBusyProgress = false;
        if (!rest.Success())
        {
            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }
        await ParentDirectoryElementsList.ReloadElements(null, true);
    }

    /// <inheritdoc/>
    protected async Task DeleteElementOfDirectory()
    {
        IsBusyProgress = true;
        await Task.Delay(1);
        ResponseBaseModel rest = await ConstructorRepo.DeleteElementFromDirectory(new() { Payload = ElementObject.Id, SenderActionUserId = CurrentUser.UserId });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }
        await ConstructorRepo.CheckAndNormalizeSortIndexForElementsOfDirectory(SelectedDirectoryId);
        await ParentDirectoryElementsList.ReloadElements(null, true);
    }
}
