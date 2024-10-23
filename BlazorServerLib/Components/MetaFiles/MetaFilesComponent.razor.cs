////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.MetaFiles;

/// <summary>
/// MetaFilesComponent
/// </summary>
public partial class MetaFilesComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService FilesRepo { get; set; } = default!;


    FilesAreaMetadataModel[] FilesAreaMetadata = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReadCurrentUser();
        await SetBusy();
        TResponseModel<FilesAreaMetadataModel[]> res = await FilesRepo.FilesAreaGetMetadata(new());
        await SetBusy(false);
        FilesAreaMetadata = res.Response ?? throw new Exception();

    }
}