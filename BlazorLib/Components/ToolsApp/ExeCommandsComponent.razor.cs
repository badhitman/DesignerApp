////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;
using BlazorLib;

namespace BlazorLib.Components.ToolsApp;

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
    public required ToolsAppMainComponent ParentPage { get; set; }

    private ExeCommandModelDB newCommand = new() { FileName = "", Arguments = "", Name = "" };
        

    async Task AddNewCommand()
    {
        //MauiProgram.ExeCommands.Response ??= [];
        //MauiProgram.ExeCommands.Response.Add(newCommand);
        //await ParentPage.HoldPageUpdate(true);
        await SetBusy();
        //await MauiProgram.SaveCommands(MauiProgram.ExeCommands.Response);
        //await ParentPage.HoldPageUpdate(false);
        //newCommand = new() { FileName = "", Arguments = "", Name = "" };
        await SetBusy(false);
        //SnackbarRepo.ShowMessagesResponse(MauiProgram.ExeCommands.Messages);
    }
}