////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using ToolsMauiApp.Components.Pages;
using SharedLib;
using BlazorLib;

namespace ToolsMauiApp.Components;

/// <summary>
/// ExeCommandsComponent
/// </summary>
public partial class ExeCommandsComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IServerToolsService ToolsLocalRepo { get; set; } = default!;

    [Inject]
    IClientHTTPRestService ToolsExtRepo { get; set; } = default!;


    /// <summary>
    /// Home page
    /// </summary>
    [Parameter, EditorRequired]
    public required Home ParentPage { get; set; }

    private ExeCommandModel newCommand = new() { FileName = "", Arguments = "" };
        

    async Task AddNewCommand()
    {
        MauiProgram.ExeCommands.Response ??= [];
        MauiProgram.ExeCommands.Response.Add(newCommand);
        await ParentPage.HoldPageUpdate(true);
        await SetBusy();
        await MauiProgram.SaveCommands(MauiProgram.ExeCommands.Response);
        await ParentPage.HoldPageUpdate(false);
        newCommand = new() { FileName = "", Arguments = "" };
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(MauiProgram.ExeCommands.Messages);
    }
}