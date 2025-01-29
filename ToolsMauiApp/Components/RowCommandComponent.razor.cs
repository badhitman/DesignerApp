////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace ToolsMauiApp.Components;

/// <summary>
/// RowCommandComponent
/// </summary>
public partial class RowCommandComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IClientHTTPRestService ToolsExtRepo { get; set; } = default!;


    /// <summary>
    /// Row index
    /// </summary>
    [Parameter, EditorRequired]
    public int RowIndex { get; set; }

    /// <summary>
    /// Owner component
    /// </summary>
    [Parameter, EditorRequired]
    public required ExeCommandsComponent OwnerComponent { get; set; }


    ExeCommandModel CurrentCommand = default!;


    async Task RunCommand()
    {
        await SetBusy();
        await OwnerComponent.SetBusy();
        TResponseModel<string> res = await ToolsExtRepo.ExeCommand(MauiProgram.ExeCommands.Response![RowIndex]);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await SetBusy(false);
        await OwnerComponent.SetBusy(false);
    }

    void CancelEdit()
    {
        CurrentCommand = GlobalTools.CreateDeepCopy(MauiProgram.ExeCommands.Response![RowIndex])!;
    }

    async Task SaveRow()
    {
        await SetBusy();
        await OwnerComponent.SetBusy();
        MauiProgram.ExeCommands.Response![RowIndex] = GlobalTools.CreateDeepCopy(CurrentCommand)!;
        await MauiProgram.SaveCommands(MauiProgram.ExeCommands.Response!);
        await OwnerComponent.SetBusy(false);
        await SetBusy(false);
    }

    async Task DeleteCommand()
    {
        await SetBusy();
        await OwnerComponent.SetBusy();
        MauiProgram.ExeCommands.Response!.RemoveAt(RowIndex);
        await MauiProgram.SaveCommands(MauiProgram.ExeCommands.Response!);
        await OwnerComponent.SetBusy(false);
        await SetBusy(false);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        CurrentCommand = GlobalTools.CreateDeepCopy(MauiProgram.ExeCommands.Response![RowIndex])!;
    }
}