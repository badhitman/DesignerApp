////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace ToolsMauiApp.Components;

public partial class RowCommandComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IToolsSystemExtService ToolsExtRepo { get; set; } = default!;


    /// <summary>
    /// RowIndex
    /// </summary>
    [Parameter, EditorRequired]
    public int RowIndex { get; set; }

    /// <summary>
    /// OwnerComponent
    /// </summary>
    [Parameter, EditorRequired]
    public required ExeCommandsComponent OwnerComponent { get; set; }

    async Task RunCommand(int i)
    {
        await SetBusy();
        await OwnerComponent.SetBusy();
        TResponseModel<string> res = await ToolsExtRepo.ExeCommand(MauiProgram.ExeCommands.Response![i]);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await SetBusy(false);
        await OwnerComponent.SetBusy(false);
    }

    void DeleteCommand(int i)
    {
        MauiProgram.ExeCommands.Response!.RemoveAt(i);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        
    }
}
