////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce.OrderDocumentObject;

/// <summary>
/// OrderDocumentObjectComponent
/// </summary>
public partial class OrderDocumentObjectComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService StorageRepo { get; set; } = default!;

    [Inject]
    NavigationManager NavRepo { get; set; } = default!;

    [Inject]
    IJSRuntime JsRuntimeRepo { get; set; } = default!;


    /// <summary>
    /// Document
    /// </summary>
    [Parameter, EditorRequired]
    public required OrderDocumentModelDB Document { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required IssueHelpdeskModelDB Issue { get; set; }


    UserInfoMainModel user = default!;

    AttachmentForOrderModelDB? _selectedFile;
    void FileManage(AttachmentForOrderModelDB _f)
    {
        isInitDelete = false;
        _selectedFile = _f;
    }

    async Task DownloadFile()
    {
        isInitDelete = false;
        if (_selectedFile is null)
            return;

        TResponseModel<byte[]> downloadSource = await CommRepo.GetFileOrder(_selectedFile.FilePoint);
        if (downloadSource.Success() && downloadSource.Response is not null)
        {
            MemoryStream ms = new(downloadSource.Response);
            using DotNetStreamReference streamRef = new(stream: ms);
            await JsRuntimeRepo.InvokeVoidAsync("downloadFileFromStream", _selectedFile.Name, streamRef);
        }
    }

    bool isInitDelete;
    async Task TryDelete()
    {
        if (_selectedFile is null || Document.Attachments is null)
            return;

        if (!isInitDelete)
        {
            isInitDelete = true;
            return;
        }

        TResponseModel<bool> del_res = await CommRepo.AttachmentDeleteFromOrder(_selectedFile.Id);

        if (del_res.Success() && del_res.Response == true)
            Document.Attachments.RemoveAll(x => x.Id == _selectedFile.Id);

        isInitDelete = false;
    }

    void closeFileManager()
    {
        _selectedFile = null;
        isInitDelete = false;
    }

    async Task OrderToCart()
    {
        OrderDocumentModelDB doc = GlobalTools.CreateDeepCopy(Document)!;

        doc.Id = 0;
        doc.Attachments = null;
        doc.ExternalDocumentId = null;
        doc.CreatedAtUTC = DateTime.UtcNow;
        doc.LastAtUpdatedUTC = DateTime.UtcNow;
        doc.HelpdeskId = null;
        doc.Name = "Новый";
        doc.Information = null;


        doc.Organization = null;
        doc.AddressesTabs!.ForEach(x =>
        {
            x.Id = 0;
            //x.AddressOrganization = null;
            x.OrderDocumentId = 0;
            x.Rows?.ForEach(y =>
            {
                y.Id = 0;
                //y.OrderDocument = doc;
                y.OrderDocumentId = 0;
                //y.Goods = null;
                //y.Offer = null;
            });
        });


        await SetBusy();
        
        TResponseModel<int> res = await StorageRepo.SaveParameter(doc, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId), true);

        SnackbarRepo.ShowMessagesResponse(res.Messages);
        SnackbarRepo.Add("Содержимое документа отправлено в корзину для формирования нового заказа", Severity.Info, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

        if (res.Success())
            NavRepo.NavigateTo("/create-order");
        else
            IsBusyProgress = false;

    }

    async Task OrderNull()
    {
        TAuthRequestModel<StatusChangeRequestModel> req = new()
        {
            SenderActionUserId = user.UserId,
            Payload = new()
            {
                IssueId = Issue.Id,
                Step = StatusesDocumentsEnum.Canceled,
            }
        };
        await SetBusy();
        
        TResponseModel<bool> res = await HelpdeskRepo.StatusChange(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Response && res.Success())
            NavRepo.ReloadPage();
    }
}