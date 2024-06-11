using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorWebLib.Components;

/// <summary>
/// Input rich text
/// </summary>
public partial class InputRichTextComponent : InputTextArea
{
    [Inject] IJSRuntime JsRuntimeRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter]
    public bool ReadOnly { get; set; } = false;

    /// <inheritdoc/>
    public readonly string UID = $"CKEditor_{Guid.NewGuid().ToString().ToLower().Replace("-", "")}";
    
    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JsRuntimeRepo.InvokeVoidAsync("CKEditorInterop.init", UID, ReadOnly, DotNetObjectReference.Create(this));
        else
            await JsRuntimeRepo.InvokeVoidAsync("CKEditorInterop.isReadOnly", UID, ReadOnly);
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task EditorDataChanged(string data)
    {
        CurrentValue = data;
        StateHasChanged();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        _ = InvokeAsync(async () => { await JsRuntimeRepo.InvokeVoidAsync("CKEditorInterop.destroy", UID); });
        base.Dispose(disposing);
    }
}