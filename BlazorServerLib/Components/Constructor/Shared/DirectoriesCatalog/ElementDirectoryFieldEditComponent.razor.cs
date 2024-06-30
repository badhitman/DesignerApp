////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using BlazorWebLib.Components.Constructor.Pages;

namespace BlazorWebLib.Components.Constructor.Shared.DirectoriesCatalog;

/// <summary>
/// Element directory field edit
/// </summary>
public partial class ElementDirectoryFieldEditComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    IConstructorService ConstructorRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required SystemEntryModel ElementObject { get; set; }
    SystemEntryModel ElementObjectEdit = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public Action EditDoneAction { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }


    /// <inheritdoc/>
    protected bool IsEdited => !ElementObject.Equals(ElementObjectEdit);

    /// <inheritdoc/>
    protected async Task UpdateElementOfDirectory()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await ConstructorRepo.UpdateElementOfDirectory(ElementObjectEdit);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        ElementObject.Update(ElementObjectEdit);
        EditDoneAction();
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ElementObjectEdit = SystemEntryModel.Build(ElementObject);
        base.OnInitialized();
    }
}