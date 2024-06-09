using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class ElementDirectoryFieldEditComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<ElementDirectoryFieldEditComponent> _logger { get; set; } = default!;

    [Inject]
    protected IJSRuntime _js_runtime { get; set; } = default!;

    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public EntryModel ElementObject { get; set; } = default!;

    [Parameter, EditorRequired]
    public Action EditDoneAction { get; set; } = default!;

    string orign_name { get; set; } = "";
    protected bool IsEdited => !orign_name.Equals(ElementObject.Name);

    protected async Task UpdateElementOfDirectory()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.UpdateElementOfDirectory(new EntryModel() { Id = ElementObject.Id, Name = orign_name });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        ElementObject.Name = orign_name;
        EditDoneAction();
    }

/// <inheritdoc/>
    protected override void OnInitialized()
    {
        orign_name = ElementObject.Name;
        base.OnInitialized();
    }
}