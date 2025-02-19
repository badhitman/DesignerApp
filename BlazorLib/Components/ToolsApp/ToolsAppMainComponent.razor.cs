////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components.ToolsApp;

public partial class ToolsAppMainComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IClientHTTPRestService RestClientRepo { get; set; } = default!;

    [Inject]
    IToolsAppManager ToolsApp { get; set; } = default!;

    [Inject]
    ApiRestConfigModelDB ApiConnect { get; set; } = default!;


    ApiRestConfigModelDB[] AllTokens = [];
    ConnectionConfigComponent configRef = default!;

    //int? _selectedOfferId;
    /// <summary>
    /// SelectedOfferId
    /// </summary>
    public int SelectedConfId
    {
        get => ApiConnect.Id;
        set => InvokeAsync(() => SetActiveHandler(value));
    }

    async void SetActiveHandler(int selectedConfId)
    {
        await SetBusy();
        if (!AllTokens.Any(x => x.Id == selectedConfId))
            AllTokens = await ToolsApp.GetAllConfigurations();

        ApiRestConfigModelDB selectedConnect = AllTokens.First(x => x.Id == selectedConfId);
        ApiConnect.Update(selectedConnect);
        await SetBusy(false);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AllTokens = await ToolsApp.GetAllConfigurations();
        SelectedConfId = AllTokens.OrderBy(x => x.Name).FirstOrDefault()?.Id ?? 0;

    }
}