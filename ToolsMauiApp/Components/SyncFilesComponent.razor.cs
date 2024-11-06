////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace ToolsMauiApp.Components;

public partial class SyncFilesComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IToolsSystemService toolsLocalRepo { get; set; } = default!;

    [Inject]
    IToolsSystemExtService toolsExtRepo { get; set; } = default!;

    async Task SyncRun()
    {
        if(string.IsNullOrWhiteSpace(MauiProgram.ConfigStore.Response?.RemoteDirectory) || string.IsNullOrWhiteSpace(MauiProgram.ConfigStore.Response.LocalDirectory))
        {
            SnackbarRepo.Error("RemoteDirectory or LocalDirectory: is empty");
            return;
        }

        await SetBusy();
        TResponseModel<ToolsFilesResponseModel[]> localScan = await toolsLocalRepo.GetDirectory(new ToolsFilesRequestModel 
        { 
            RemoteDirectory = MauiProgram.ConfigStore.Response.LocalDirectory,
        });
        TResponseModel<ToolsFilesResponseModel[]> remoteScan = await toolsLocalRepo.GetDirectory(new ToolsFilesRequestModel 
        { 
            RemoteDirectory = MauiProgram.ConfigStore.Response.RemoteDirectory,
        });
        await SetBusy(false);
    }
}