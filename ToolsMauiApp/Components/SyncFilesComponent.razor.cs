////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Microsoft.AspNetCore.Components;
using SharedLib;
using System.IO.Compression;
using System.Security.Cryptography;

namespace ToolsMauiApp.Components;

/// <summary>
/// SyncFilesComponent
/// </summary>
public partial class SyncFilesComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IToolsSystemService ToolsLocalRepo { get; set; } = default!;

    [Inject]
    IToolsSystemExtService ToolsExtRepo { get; set; } = default!;


    TResponseModel<List<ToolsFilesResponseModel>>? localScan;
    bool localScanBusy;

    TResponseModel<List<ToolsFilesResponseModel>>? remoteScan;
    bool remoteScanBusy;

    ToolsFilesResponseModel[]? forDelete;
    ToolsFilesResponseModel[]? forUpdateOrAdd;

    async Task SyncRun()
    {
        if (string.IsNullOrWhiteSpace(MauiProgram.ConfigStore.Response?.RemoteDirectory) || string.IsNullOrWhiteSpace(MauiProgram.ConfigStore.Response.LocalDirectory))
        {
            SnackbarRepo.Error("RemoteDirectory or LocalDirectory: is empty");
            return;
        }
        forDelete = null;
        forUpdateOrAdd = null;
        await Task.WhenAll([ReadLocalData(), ReadRemoteData()]);
        if (localScan?.Response is null || remoteScan?.Response is null)
        {
            SnackbarRepo.Error("localScan is null || remoteScan is null");
            return;
        }
        await SetBusy();
        forDelete = remoteScan.Response
            .Where(x => !localScan.Response.Any(y => x.SafeScopeName == y.SafeScopeName))
            .ToArray();

        forUpdateOrAdd = [.. localScan.Response
            .Where(x => !remoteScan.Response.Any(y => x.SafeScopeName == y.SafeScopeName))
            .Union(remoteScan.Response.Where(x => localScan.Response.Any(y => x.SafeScopeName == y.SafeScopeName && !x.Equals(y))))
            .OrderByDescending(x => x.Size)];

        await SetBusy(false);
    }

    async Task Send()
    {
        if (forDelete is null || forUpdateOrAdd is null)
        {
            SnackbarRepo.Error("forDelete is null || forUpdateOrAdd is null");
            return;
        }

        if (string.IsNullOrWhiteSpace(MauiProgram.ConfigStore.Response?.RemoteDirectory))
        {
            SnackbarRepo.Error("RemoteDirectory is empty");
            return;
        }

        if (string.IsNullOrWhiteSpace(MauiProgram.ConfigStore.Response?.LocalDirectory))
        {
            SnackbarRepo.Error("LocalDirectory is empty");
            return;
        }

        if (forDelete.Length == 0 && forUpdateOrAdd.Length == 0)
            return;

        await SetBusy();
        MemoryStream ms;

        if (forDelete.Length != 0)
            foreach (ToolsFilesResponseModel tFile in forDelete)
                await ToolsExtRepo.DeleteFile(new DeleteRemoteFileRequestModel()
                {
                    RemoteDirectory = MauiProgram.ConfigStore.Response.RemoteDirectory,
                    SafeScopeName = tFile.SafeScopeName,
                });

        using MD5 md5 = MD5.Create();
        string _hash;
        long totalTransferData = 0;
        if (forUpdateOrAdd.Length != 0)
            foreach (ToolsFilesResponseModel tFile in forUpdateOrAdd)
            {
                try
                {
                    string archive = Path.GetTempFileName();
                    using ZipArchive zip = ZipFile.Open(archive, ZipArchiveMode.Update);

                    string _fnT = Path.Combine(MauiProgram.ConfigStore.Response.LocalDirectory, tFile.SafeScopeName);

                    ZipArchiveEntry entry = zip.CreateEntryFromFile(_fnT, tFile.SafeScopeName);
                    zip.Dispose();
                    ms = new MemoryStream(File.ReadAllBytes(archive));
                    TResponseModel<string> resUpd = await ToolsExtRepo.UpdateFile(tFile.SafeScopeName, MauiProgram.ConfigStore.Response.RemoteDirectory, ms.ToArray());

                    if (resUpd.Messages.Any(x => x.TypeMessage == ResultTypesEnum.Error || x.TypeMessage >= ResultTypesEnum.Info))
                        SnackbarRepo.ShowMessagesResponse(resUpd.Messages);

                    totalTransferData += ms.Length;

                    using FileStream stream = File.OpenRead(_fnT);
                    _hash = Convert.ToBase64String(md5.ComputeHash(stream));

                    if (_hash != resUpd.Response)
                        SnackbarRepo.Error($"Hash file conflict `{tFile.FullName}`: L[{_hash}]{_fnT} R[{Path.Combine(tFile.SafeScopeName, MauiProgram.ConfigStore.Response.RemoteDirectory)}]");

                    File.Delete(archive);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        await SyncRun();
        if (totalTransferData != 0)
            SnackbarRepo.Add($"Отправлено: {GlobalTools.SizeDataAsString(totalTransferData)}", MudBlazor.Severity.Info, c => c.DuplicatesBehavior = MudBlazor.SnackbarDuplicatesBehavior.Allow);
    }

    async Task<FileSaverResult?> PickAndShow(MemoryStream ms)
    {
        FileSaverResult fileSaverResult = await FileSaver.Default.SaveAsync("build-update.zip", ms);
        if (fileSaverResult.IsSuccessful)
        {
            await Toast.Make($"Файл успешно сохранен в указанном месте: {fileSaverResult.FilePath}").Show();
        }
        else
        {
            await Toast.Make($"Файл не был успешно сохранен из-за ошибки: {fileSaverResult.Exception.Message}").Show();
        }

        return fileSaverResult;
    }

    async Task ReadLocalData()
    {
        localScanBusy = true;
        await Task.Delay(1);
        StateHasChanged();
        localScan = await ToolsLocalRepo.GetDirectory(new ToolsFilesRequestModel
        {
            RemoteDirectory = MauiProgram.ConfigStore.Response!.LocalDirectory!,
            CalculationVersion = true,
            CalculationHash = true,
        });
        localScanBusy = false;
    }

    async Task ReadRemoteData()
    {
        remoteScanBusy = true;
        await Task.Delay(1);
        StateHasChanged();
        remoteScan = await ToolsExtRepo.GetDirectory(new ToolsFilesRequestModel
        {
            RemoteDirectory = MauiProgram.ConfigStore.Response!.RemoteDirectory!,
            CalculationVersion = true,
            CalculationHash = true,
        });
        remoteScanBusy = false;
    }
}