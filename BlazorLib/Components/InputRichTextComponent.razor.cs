using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorLib.Components;

public partial class InputRichTextComponent : InputTextArea
{
    [Inject] IJSRuntime _js_runtime { get; set; } = default!;

    [Parameter]
    public bool ReadOnly { get; set; } = false;

    public readonly string UID = $"CKEditor_{Guid.NewGuid().ToString().ToLower().Replace("-", "")}";
    //string rendered_val = string.Empty;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _js_runtime.InvokeVoidAsync("CKEditorInterop.init", UID, ReadOnly, DotNetObjectReference.Create(this));
        }
        else
        {
            await _js_runtime.InvokeVoidAsync("CKEditorInterop.isReadOnly", UID, ReadOnly);
        }
    }

    [JSInvokable]
    public Task EditorDataChanged(string data)
    {
        CurrentValue = data;
        StateHasChanged();
        return Task.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        _ = InvokeAsync(async () => { await _js_runtime.InvokeVoidAsync("CKEditorInterop.destroy", UID); });
        base.Dispose(disposing);
    }
}