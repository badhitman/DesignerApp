using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Element directory field edit
/// </summary>
public partial class ElementDirectoryFieldEditComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected IJSRuntime JsRuntimeRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public SystemEntryModel ElementObject { get; set; } = default!;
    SystemEntryModel ElementObjectEdit = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public Action EditDoneAction { get; set; } = default!;

    /// <inheritdoc/>
    protected bool IsEdited => !ElementObject.Equals(ElementObjectEdit);

    /// <inheritdoc/>
    protected async Task UpdateElementOfDirectory()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.UpdateElementOfDirectory(ElementObjectEdit);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        ElementObject.Update(ElementObjectEdit);
        EditDoneAction();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ElementObjectEdit = SystemEntryModel.Build(ElementObject);
        base.OnInitialized();
    }
}