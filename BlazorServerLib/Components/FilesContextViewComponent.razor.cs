////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components;

/// <summary>
/// FilesContextViewComponent
/// </summary>
public partial class FilesContextViewComponent : MetaPropertyBaseComponent
{
    [Inject]
    NavigationManager NavRepo { get; set; } = default!;

    [Inject]
    IJSRuntime JsRuntimeRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService FilesRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HdRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommRepo { get; set; } = default!;


    bool CanAddingFile => OwnerPrimaryKey.HasValue && OwnerPrimaryKey.Value > 0 &&
        !string.IsNullOrWhiteSpace(PrefixPropertyName) &&
        !string.IsNullOrWhiteSpace(PropertyName) &&
        ApplicationsNames.Length == 1;

    private string? searchString = null;
    private string _inputFileId = Guid.NewGuid().ToString();
    private readonly List<IBrowserFile> loadedFiles = [];

    /// <summary>
    /// Table (ref)
    /// </summary>
    MudTable<StorageFileModelDB>? TableRef;

    /// <summary>
    /// ReloadServerData
    /// </summary>
    public async Task ReloadServerData()
    {
        if (TableRef is not null)
            await TableRef.ReloadServerData();
    }

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

    async Task SendFile()
    {
        if (loadedFiles.Count == 0)
            throw new Exception();
        string appName = ApplicationsNames.Single();
        StorageImageMetadataModel req = new()
        {
            AuthorUserIdentity = CurrentUserSession!.UserId,
            PrefixPropertyName = PrefixPropertyName,
            ApplicationName = appName,
            OwnerPrimaryKey = OwnerPrimaryKey,
            PropertyName = PropertyName,
            Referrer = NavRepo.Uri,
            //
            FileName = "",
            ContentType = "",
            Payload = []
        };

        await SetBusy();

        TResponseModel<StorageFileModelDB> res;
        MemoryStream ms;

        foreach (IBrowserFile fileBrowser in loadedFiles)
        {
            ms = new();
            await fileBrowser.OpenReadStream(maxAllowedSize: 1024 * 18 * 1024).CopyToAsync(ms);

            req.Payload = ms.ToArray();
            req.ContentType = fileBrowser.ContentType;
            req.FileName = fileBrowser.Name;

            await ms.DisposeAsync();
            res = await FilesRepo.SaveFile(new() { Payload = req, SenderActionUserId = CurrentUserSession.UserId });
            SnackbarRepo.ShowMessagesResponse(res.Messages);
        }

        loadedFiles.Clear();
        _inputFileId = Guid.NewGuid().ToString();
        await SetBusy(false);

        if (TableRef is not null)
            await TableRef.ReloadServerData();
    }

    /// <inheritdoc/>
    protected async Task ClipboardCopyHandle()
    {
        if (_selectedFile is null)
            return;

        await JsRuntimeRepo.InvokeVoidAsync("clipboardCopy.copyText", $"{NavRepo.BaseUri}cloud-fs/read/{_selectedFile.Id}/{_selectedFile.FileName}");
        SnackbarRepo.Add($"Ссылка {_selectedFile.FileName} скопирована в буфер обмена", Severity.Info, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
    }

    async Task DownloadFile()
    {
        if (_selectedFile is null || CurrentUserSession is null)
            return;

        TResponseModel<FileContentModel> downloadSource = await FilesRepo.ReadFile(new TAuthRequestModel<RequestFileReadModel>() { SenderActionUserId = CurrentUserSession.UserId, Payload = new() { FileId = _selectedFile.Id } });

        if (downloadSource.Success() && downloadSource.Response?.Payload is not null)
        {
            MemoryStream ms = new(downloadSource.Response.Payload);
            using DotNetStreamReference streamRef = new(stream: ms);
            await JsRuntimeRepo.InvokeVoidAsync("downloadFileFromStream", _selectedFile.FileName, streamRef);
        }
    }

    private async Task<TableData<StorageFileModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        IsBusyProgress = true;
        TPaginationRequestModel<SelectMetadataRequestModel> req = new()
        {
            Payload = new()
            {
                SearchQuery = searchString,
                ApplicationsNames = ApplicationsNames,
                IdentityUsersIds = [],
                PropertyName = ManageMode ? "" : PropertyName,
                OwnerPrimaryKey = OwnerPrimaryKey,
                PrefixPropertyName = ManageMode ? "" : PrefixPropertyName,
            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };

        TPaginationResponseModel<StorageFileModelDB> rest = await FilesRepo
            .FilesSelect(req);

        List<StorageFileModelDB>? data = rest.Response;
        IsBusyProgress = false;
        return new() { TotalItems = rest.TotalRowsCount, Items = data };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        if (TableRef is not null)
            InvokeAsync(TableRef.ReloadServerData);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReadCurrentUser();
        await SetBusy(false);
    }
}