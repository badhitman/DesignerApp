////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
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


    ConfigStoreModel configEdit = new();
    TResponseModel<ExpressProfileResponseModel>? testResult;
    TResponseModel<List<ToolsFilesResponseModel>>? checkDir;

    bool CanSave => true;//configEdit.FullSets && !configEdit.Equals(MauiProgram.ConfigStore.Response);

    bool HoldPage = false;

    /// <summary>
    /// HoldPageUpdate
    /// </summary>
    public async Task HoldPageUpdate(bool _set)
    {
        HoldPage = _set;
        await Task.Delay(1);
        StateHasChanged();
    }

    static string ColorStatus(ResultTypesEnum rt) => rt switch
    {
        ResultTypesEnum.Error => "danger",
        ResultTypesEnum.Success => "success",
        ResultTypesEnum.Info => "info",
        ResultTypesEnum.Alert => "primary",
        ResultTypesEnum.Warning => "warning",
        _ => throw new NotImplementedException(),
    };

    async Task SaveConfig()
    {
        //await SetBusy();
        //await MauiProgram.SaveConfig(configEdit);
        //await SetBusy(false);
        //SnackbarRepo.ShowMessagesResponse(MauiProgram.ConfigStore.Messages);
    }

    async Task TestConnect()
    {
        //if (string.IsNullOrWhiteSpace(configEdit.LocalDirectory) || string.IsNullOrWhiteSpace(configEdit.RemoteDirectory))
        //{
        //    SnackbarRepo.Error("Не установлена директория");
        //    return;
        //}

        //DirectoryInfo _fi = new(configEdit.LocalDirectory);
        //if (!_fi.Exists)
        //{
        //    SnackbarRepo.Error("Локальная директория отсутствует");
        //    return;
        //}

        //MauiProgram.ConfigStore.Messages.Clear();
        //ToolsFilesRequestModel req = new()
        //{
        //    RemoteDirectory = configEdit.RemoteDirectory,
        //};

        //await SetBusy();

        //testResult = await RestClientRepo.GetMe();
        //SnackbarRepo.ShowMessagesResponse(testResult.Messages);
        //checkDir = await RestClientRepo.GetDirectory(req);
        //SnackbarRepo.ShowMessagesResponse(checkDir.Messages);

        //await SetBusy(false);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        //ApiRestConfigModelDB[] tokens = await ToolsApp.GetAllConfigurations();

        //if (MauiProgram.ConfigStore.Response is null)
        //{
        //    MauiProgram.ConfigStore.AddError("MauiProgram.ConfigStore.Response is null");
        //    return;
        //}

        //configEdit = GlobalTools.CreateDeepCopy(MauiProgram.ConfigStore.Response)!;

        //if (MauiProgram.ConfigStore.Response.FullSets)
        //{
        //    await SetBusy();
        //    testResult = await RestClientRepo.GetMe();
            
        //    await SetBusy(false);
        //}
    }
}