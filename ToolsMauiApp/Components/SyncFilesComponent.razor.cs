////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using ToolsMauiApp.Components.Pages;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Alerts;
using System.Security.Cryptography;
using System.IO.Compression;
using SharedLib;
using Microsoft.Extensions.Logging;

namespace ToolsMauiApp.Components;

/// <summary>
/// SyncFilesComponent
/// </summary>
public partial class SyncFilesComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ILogger<SyncFilesComponent> LoggerRepo { get; set; } = default!;

    [Inject]
    IToolsSystemService ToolsLocalRepo { get; set; } = default!;

    [Inject]
    IToolsSystemHTTPRestService ToolsExtRepo { get; set; } = default!;


    /// <summary>
    /// Home page
    /// </summary>
    [Parameter, EditorRequired]
    public required Home ParentPage { get; set; }


    string? InfoAbout;

    private string searchStringQuery = "";

    TResponseModel<List<ToolsFilesResponseModel>>? localScan;
    bool localScanBusy;

    TResponseModel<List<ToolsFilesResponseModel>>? remoteScan;
    bool remoteScanBusy;

    ToolsFilesResponseModel[]? forDelete;
    ToolsFilesResponseModel[]? forUpdateOrAdd;
    bool IndeterminateProgress;
    public double ValueProgress { get; set; }
    long forUpdateOrAddSum;

    private bool FilterFunc1(ToolsFilesResponseModel element) => SyncFilesComponent.FilterFunc(element, searchStringQuery);

    private static bool FilterFunc(ToolsFilesResponseModel element, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Hash?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        return false;
    }

    async Task SyncRun()
    {
        if (string.IsNullOrWhiteSpace(MauiProgram.ConfigStore.Response?.RemoteDirectory) || string.IsNullOrWhiteSpace(MauiProgram.ConfigStore.Response.LocalDirectory))
        {
            SnackbarRepo.Error("RemoteDirectory or LocalDirectory: is empty");
            return;
        }

        IndeterminateProgress = true;
        await ParentPage.HoldPageUpdate(true);
        await SetBusy();

        forDelete = null;
        forUpdateOrAdd = null;

        await Task.WhenAll([ReadLocalData(), ReadRemoteData()]);
        await ParentPage.HoldPageUpdate(false);
        await SetBusy(false);

        if (localScan?.Response is null || remoteScan?.Response is null)
        {
            SnackbarRepo.Error("localScan is null || remoteScan is null");
            return;
        }

        forDelete = remoteScan.Response
            .Where(x => !localScan.Response.Any(y => x.SafeScopeName == y.SafeScopeName))
            .ToArray();

        forUpdateOrAdd = [.. localScan.Response
            .Where(x => !remoteScan.Response.Any(y => x.SafeScopeName == y.SafeScopeName))
            .Union(remoteScan.Response.Where(x => localScan.Response.Any(y => x.SafeScopeName == y.SafeScopeName && !x.Equals(y))))
            .OrderByDescending(x => x.Size)];

        forUpdateOrAddSum = forUpdateOrAdd.Sum(x => x.Size);
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

        IndeterminateProgress = true;
        ValueProgress = 0;
        await ParentPage.HoldPageUpdate(true);
        await SetBusy();

        MemoryStream ms;

        if (forDelete.Length != 0)
        {
            InfoAbout = "Удаление файлов...";
            foreach (ToolsFilesResponseModel tFile in forDelete)
                await ToolsExtRepo.DeleteFile(new DeleteRemoteFileRequestModel()
                {
                    RemoteDirectory = MauiProgram.ConfigStore.Response.RemoteDirectory,
                    SafeScopeName = tFile.SafeScopeName,
                });
            InfoAbout = $"Удалено файлов: {forDelete.Length} шт.";
        }

        using MD5 md5 = MD5.Create();
        string _hash;
        long totalTransferData = 0, totalReadData = 0;
        IndeterminateProgress = false;
        StateHasChanged();
        if (forUpdateOrAdd.Length != 0)
        {
            InfoAbout = "Отправка файлов...";
            int _cntFiles = 0;
            foreach (ToolsFilesResponseModel tFile in forUpdateOrAdd)//.OrderBy(x => x.Size)
            {
                _cntFiles++;
                totalReadData += tFile.Size;
                try
                {
                    string archive = Path.GetTempFileName();
                    using ZipArchive zip = ZipFile.Open(archive, ZipArchiveMode.Update);

                    string _fnT = Path.Combine(MauiProgram.ConfigStore.Response.LocalDirectory, tFile.SafeScopeName);

                    ZipArchiveEntry entry = zip.CreateEntryFromFile(_fnT, tFile.SafeScopeName);
                    zip.Dispose();
                    ms = new MemoryStream(File.ReadAllBytes(archive));

                    TResponseModel<PartUploadSessionModel> sessionPartUpload = await ToolsExtRepo.PartUploadSessionStart(new PartUploadSessionStartRequestModel()
                    {
                        RemoteDirectory = MauiProgram.ConfigStore.Response.RemoteDirectory,
                        FileSize = ms.Length,
                        FileName = tFile.SafeScopeName,
                    });

                    if (sessionPartUpload.Response is null || !sessionPartUpload.Success())
                    {
                        SnackbarRepo.Error($"Ошибка открытия сессии отправки файла: {sessionPartUpload.Message()}");
                        return;
                    }

                    totalTransferData += ms.Length;
                    ValueProgress = totalReadData / (forUpdateOrAddSum / 100);
                    InfoAbout = $"Отправлено файлов: {_cntFiles} шт. (~{GlobalTools.SizeDataAsString(totalReadData)} zip:{GlobalTools.SizeDataAsString(totalTransferData)})";

                    if (sessionPartUpload.Response.FilePartsMetadata.Count == 1)
                    {
                        TResponseModel<string> resUpd = await ToolsExtRepo.UpdateFile(tFile.SafeScopeName, MauiProgram.ConfigStore.Response.RemoteDirectory, ms.ToArray());

                        if (resUpd.Messages.Any(x => x.TypeMessage == ResultTypesEnum.Error || x.TypeMessage >= ResultTypesEnum.Info))
                            SnackbarRepo.ShowMessagesResponse(resUpd.Messages);

                        using FileStream stream = File.OpenRead(_fnT);
                        _hash = Convert.ToBase64String(md5.ComputeHash(stream));

                        if (_hash != resUpd.Response)
                            SnackbarRepo.Error($"Hash file conflict `{tFile.FullName}`: L[{_hash}]{_fnT} R[{Path.Combine(tFile.SafeScopeName, MauiProgram.ConfigStore.Response.RemoteDirectory)}]");
                    }
                    else
                    {
                        foreach (FilePartMetadataModel fileMd in sessionPartUpload.Response.FilePartsMetadata)
                        {
                            ms.Position = fileMd.PartFilePositionStart;
                            byte[] _buff = new byte[fileMd.PartFileSize];
                            ms.Read(_buff, 0, _buff.Length);
                            ResponseBaseModel _subRest = await ToolsExtRepo.PartUpload(new SessionFileRequestModel(sessionPartUpload.Response.SessionId, fileMd.PartFileId, _buff, Path.GetFileName(tFile.FullName), fileMd.PartFileIndex));
                            if (!_subRest.Success())
                                SnackbarRepo.ShowMessagesResponse(_subRest.Messages);

                            StateHasChanged();
                        }
                    }

                    File.Delete(archive);
                }
                catch (Exception ex)
                {
                    SnackbarRepo.Add(ex.Message, MudBlazor.Severity.Error, c => c.DuplicatesBehavior = MudBlazor.SnackbarDuplicatesBehavior.Allow);
                    LoggerRepo.LogError(ex, $"Ошибка отправки порции данных: {tFile}");
                }
                StateHasChanged();
            }
        }

        await SyncRun();
        await ParentPage.HoldPageUpdate(false);

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
            CalculationHash = true,
        });
        remoteScanBusy = false;
    }
}