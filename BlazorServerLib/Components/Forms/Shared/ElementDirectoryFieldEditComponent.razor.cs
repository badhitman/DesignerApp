////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using BlazorWebLib.Components.Forms.Pages;

namespace BlazorWebLib.Components.Forms.Shared;

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
    IFormsService FormsRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required SystemEntryModel ElementObject { get; set; }
    SystemEntryModel ElementObjectEdit = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public Action EditDoneAction { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }


    /// <inheritdoc/>
    protected bool IsEdited => !ElementObject.Equals(ElementObjectEdit);

    /// <inheritdoc/>
    protected async Task UpdateElementOfDirectory()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.UpdateElementOfDirectory(ElementObjectEdit, CurrentUser.UserId);
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