////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;
using static MudBlazor.CategoryTypes;
using System.Net.Http;
using Microsoft.JSInterop;

namespace BlazorWebLib.Components;

/// <summary>
/// FilesContextViewComponent
/// </summary>
public partial class FilesContextViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IJSRuntime JsRuntimeRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService FilesRepo { get; set; } = default!;


    /// <summary>
    /// Приложение
    /// </summary>
    [Parameter, EditorRequired]
    public required string ApplicationName { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    [Parameter, EditorRequired]
    public required string PropertyName { get; set; }

    /// <summary>
    /// Префикс
    /// </summary>
    [Parameter]
    public string? PrefixPropertyName { get; set; }

    /// <summary>
    /// Идентификатор [PK] владельца объекта
    /// </summary>
    [Parameter]
    public int? OwnerPrimaryKey { get; set; }


    private MudTable<StorageFileModelDB>? table;
    StorageFileModelDB? _selectedFile;
    void FileManage(StorageFileModelDB _f)
    {
        _selectedFile = _f;
    }
    void closeFileManager()
    {
        _selectedFile = null;
    }

    private string? searchString = null;

    async Task DownloadFile()
    {
        if (_selectedFile is null)
            return;

        TResponseModel<StorageFileResponseModel> downloadSource = await FilesRepo.ReadFile(_selectedFile.Id);

        if (downloadSource.Success() && downloadSource.Response?.Payload is not null)
        {
            MemoryStream ms = new(downloadSource.Response.Payload);
            using DotNetStreamReference streamRef = new(stream: ms);
            await JsRuntimeRepo.InvokeVoidAsync("downloadFileFromStream", _selectedFile.FileName, streamRef);
        }
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<StorageFileModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        await SetBusy(token: token);
        TPaginationRequestModel<SelectFilesRequestModel> req = new()
        {
            Payload = new()
            {
                SearchQuery = searchString,
                IncludeExternal = false,
                ApplicationName = ApplicationName,
                IdentityUsersIds = [],
                PropertyName = PropertyName,
                OwnerPrimaryKey = OwnerPrimaryKey,
                PrefixPropertyName = PrefixPropertyName,
            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };

        TResponseModel<TPaginationResponseModel<StorageFileModelDB>> rest = await FilesRepo
            .FilesSelect(req);

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        List<StorageFileModelDB> data = rest.Response!.Response!;
        return new() { TotalItems = rest.Response.TotalRowsCount, Items = data };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table?.ReloadServerData();
    }
}