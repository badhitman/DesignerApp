////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorLib;
using SharedLib;
using MudBlazor;
using BlazorWebLib.Components.Telegram;
using Microsoft.AspNetCore.Components.Forms;

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


    private string? searchString = null;

    private readonly List<IBrowserFile> loadedFiles = [];

    private MudTable<StorageFileModelDB>? table;
    StorageFileModelDB? _selectedFile;
    void FileManage(StorageFileModelDB _f)
    {
        _selectedFile = _f;
    }
    void CloseFileManager()
    {
        _selectedFile = null;
    }

    void SelectFilesChange(InputFileChangeEventArgs e)
    {
        loadedFiles.Clear();

        foreach (IBrowserFile file in e.GetMultipleFiles())
        {
            loadedFiles.Add(file);
        }
    }

    async Task SendMessage()
    {
        if (loadedFiles.Count == 0)
            throw new Exception();

        await SetBusy();
        //SendTextMessageTelegramBotModel req = new() { Message = _textSendMessage, UserTelegramId = Chat.ChatTelegramId, From = "Техподдержка" };

        //// await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
        MemoryStream ms;


        //    req.Files = [];

        //    foreach (var fileBrowser in loadedFiles)
        //    {
        //        ms = new();
        //        await fileBrowser.OpenReadStream(maxAllowedSize: 1024 * 18 * 1024).CopyToAsync(ms);
        //        req.Files.Add(new() { ContentType = fileBrowser.ContentType, Name = fileBrowser.Name, Data = ms.ToArray() });
        //        await ms.DisposeAsync();
        //    }


        //TResponseModel<MessageComplexIdsModel?> rest = await TelegramRepo.SendTextMessageTelegram(req);
        //_textSendMessage = "";
        //if (_messagesTelegramComponent.TableRef is not null)
        //    await _messagesTelegramComponent.TableRef.ReloadServerData();

        //IsBusyProgress = false;
        //SnackbarRepo.ShowMessagesResponse(rest.Messages);
        //loadedFiles.Clear();
        //_inputFileId = Guid.NewGuid().ToString();
        //if (SendMessageHandle is not null)
        //    SendMessageHandle(req);
    }

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