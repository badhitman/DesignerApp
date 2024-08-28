////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using static MudBlazor.CategoryTypes;

namespace BlazorWebLib.Components.Constructor.Shared.DirectoriesCatalog;

/// <summary>
/// ElementDirectoryFieldSet
/// </summary>
public partial class ElementDirectoryFieldSetComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IConstructorService ConstructorRepo { get; set; } = default!;


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


    /// <summary>
    /// Current Template InputRichText ref
    /// </summary>
    protected InputRichTextComponent? _currentTemplateInputRichText_ref;

    /// <inheritdoc/>
    protected bool IsEdited => ElementObjectOrign is not null && !ElementObjectOrign.Equals(ElementObjectEdit);


    /// <inheritdoc/>
    protected async Task UpdateElementOfDirectory()
    {
        ArgumentNullException.ThrowIfNull(ElementObjectEdit);

        IsBusyProgress = true;
        ResponseBaseModel rest = await ConstructorRepo.UpdateElementOfDirectory(ElementObjectEdit);
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
        ElementObjectOrign = await ConstructorRepo.GetElementOfDirectory(ElementObject.Id);
        ElementObjectEdit = GlobalTools.CreateDeepCopy(ElementObjectOrign);
        IsBusyProgress = false;
    }

    void RsetEdit()
    {
        ElementObjectEdit = GlobalTools.CreateDeepCopy(ElementObjectOrign);
        _currentTemplateInputRichText_ref?.SetValue(ElementObjectEdit?.Description);
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
        ResponseBaseModel rest = await ConstructorRepo.UpMoveElementOfDirectory(ElementObject.Id);
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
        ResponseBaseModel rest = await ConstructorRepo.DownMoveElementOfDirectory(ElementObject.Id);
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
        ResponseBaseModel rest = await ConstructorRepo.DeleteElementFromDirectory(ElementObject.Id);
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
