﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace ToolsMauiApp.Components.Pages;

/// <summary>
/// Home
/// </summary>
public partial class Home : BlazorBusyComponentBaseModel
{
    [Inject]
    IToolsSystemExtService toolsRepo { get; set; } = default!;

    ConfigStoreModel configEdit = new();
    TResponseModel<ExpressProfileResponseModel>? testResult;
    TResponseModel<ToolsFilesResponseModel[]>? checkDir;

    bool CanSave => configEdit.FullSets && !configEdit.Equals(MauiProgram.ConfigStore.Response);

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
        await SetBusy();
        await MauiProgram.SaveConfig(configEdit);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(MauiProgram.ConfigStore.Messages);
    }

    async Task TestConnect()
    {
        if (string.IsNullOrWhiteSpace(configEdit.LocalDirectory) || string.IsNullOrWhiteSpace(configEdit.RemoteDirectory))
        {
            SnackbarRepo.Error("Не установлена директория");
            return;
        }

        DirectoryInfo _fi = new(configEdit.LocalDirectory);
        if (!_fi.Exists)
        {
            SnackbarRepo.Error("Локальная директория отсутствует");
            return;
        }

        MauiProgram.ConfigStore.Messages.Clear();
        ToolsFilesRequestModel req = new()
        {
            RemoteDirectory = configEdit.RemoteDirectory,
        };

        await SetBusy();

        testResult = await toolsRepo.GetMe();
        SnackbarRepo.ShowMessagesResponse(testResult.Messages);
        checkDir = await toolsRepo.GetDirectory(req);
        SnackbarRepo.ShowMessagesResponse(checkDir.Messages);

        await SetBusy(false);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (MauiProgram.ConfigStore.Response is null)
        {
            MauiProgram.ConfigStore.AddError("MauiProgram.ConfigStore.Response is null");
            return;
        }

        configEdit = GlobalTools.CreateDeepCopy(MauiProgram.ConfigStore.Response)!;

        if (MauiProgram.ConfigStore.Response.FullSets)
        {
            await SetBusy();
            testResult = await toolsRepo.GetMe();
            //if (testResult.Messages.Count != 0)
            //    MauiProgram.ConfigStore.Messages.AddRange(testResult.Messages);
            await SetBusy(false);
        }
    }
}