using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

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
    public EntryModel ElementObject { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public Action EditDoneAction { get; set; } = default!;

    string OriginName { get; set; } = "";
    /// <inheritdoc/>
    protected bool IsEdited => !OriginName.Equals(ElementObject.Name);

    /// <inheritdoc/>
    protected async Task UpdateElementOfDirectory()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.UpdateElementOfDirectory(new EntryModel() { Id = ElementObject.Id, Name = OriginName });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        ElementObject.Name = OriginName;
        EditDoneAction();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        OriginName = ElementObject.Name;
        base.OnInitialized();
    }
}