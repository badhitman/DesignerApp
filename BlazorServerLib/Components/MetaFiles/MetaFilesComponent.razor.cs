////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using Newtonsoft.Json;

namespace BlazorWebLib.Components.MetaFiles;

/// <summary>
/// MetaFilesComponent
/// </summary>
public partial class MetaFilesComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService FilesRepo { get; set; } = default!;

    [Inject]
    NavigationManager NavRepo { get; set; } = default!;


    /// <summary>
    /// SelectedAreas
    /// </summary>
    [Parameter]
    public string[]? SelectedAreas { get; set; }



    FilesAreaMetadataModel[] FilesAreaMetadata = [];

    private List<bool> _included = [];

    List<string>? reqNamesApps = null;

    string? _reqKey;
    /// <inheritdoc/>
    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
        {
            string reqKey = reqNamesApps is null ? "" : JsonConvert.SerializeObject(reqNamesApps);
            if (reqKey != _reqKey && reqNamesApps is not null && reqNamesApps.Count != 0)
                NavRepo.NavigateTo($"/meta-files/home/{JsonConvert.SerializeObject(reqNamesApps)}");
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        _reqKey = SelectedAreas is null || SelectedAreas.Length == 0 ? null : JsonConvert.SerializeObject(SelectedAreas);
        await ReadCurrentUser();
        await SetBusy();
        TResponseModel<FilesAreaMetadataModel[]> res = await FilesRepo.FilesAreaGetMetadata(new());
        await SetBusy(false);
        FilesAreaMetadata = res.Response ?? throw new Exception();
        _included = FilesAreaMetadata.Select(x => SelectedAreas?.Any(y => y == x.ApplicationName) == true).ToList();
    }
}