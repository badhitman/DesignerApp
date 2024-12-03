////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using Microsoft.JSInterop;

namespace BlazorWebLib.Components.ParametersShared;

/// <summary>
/// TEXTAREA - Хранимый параметр
/// </summary>
public partial class TextareaParameterStorageComponent : StringParameterStorageBaseComponent
{
    [Inject]
    IJSRuntime JsRuntimeRepo { get; set; } = default!;


    /// <summary>
    /// RowsSize
    /// </summary>
    [Parameter]
    public int RowsSize { get; set; } = 3;


    private void HandleOnChange(ChangeEventArgs args)
    {
        TextValue = args.Value?.ToString();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await JsRuntimeRepo.InvokeVoidAsync("autoGrowManage.registerGrow", KeyStorage.ToString(), DotNetObjectReference.Create(this));
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task EditorDataChanged(decimal data)
    {
        //CurrentValue = data;
        StateHasChanged();
        return Task.CompletedTask;
    }
}