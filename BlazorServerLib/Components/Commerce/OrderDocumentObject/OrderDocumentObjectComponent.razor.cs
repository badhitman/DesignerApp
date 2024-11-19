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

    bool ShowingAttachmentsOrderArea;
    List<RubricIssueHelpdeskModelDB> currentWarehouses = default!;

    async Task OrderToCart()
    {
        OrderDocumentModelDB doc = GlobalTools.CreateDeepCopy(Document)!;

        doc.Id = 0;
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

        TResponseModel<int> res = await StorageRepo.SaveParameter(doc, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(CurrentUserSession!.UserId), true);

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
            SenderActionUserId = CurrentUserSession!.UserId,
            Payload = new()
            {
                IssueId = Issue.Id,
                Step = StatusesDocumentsEnum.Canceled,
            }
        };
        await SetBusy();

        TResponseModel<bool> res = await HelpdeskRepo.StatusChange(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Response && res.Success())
            NavRepo.ReloadPage();

        await SetBusy(false);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReadCurrentUser();
        int[] orderWarehouses = [.. Document.AddressesTabs!.Select(x => x.WarehouseId).Distinct()];
        await SetBusy();

        TResponseModel<List<RubricIssueHelpdeskModelDB>?> getWarehouses = await HelpdeskRepo.RubricsGet(orderWarehouses);
        SnackbarRepo.ShowMessagesResponse(getWarehouses.Messages);
        currentWarehouses = getWarehouses.Response ?? [];

        TResponseModel<bool?> res = await StorageRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ShowingAttachmentsOrderArea);
        if (!res.Success())
            SnackbarRepo.ShowMessagesResponse(res.Messages);

        ShowingAttachmentsOrderArea = res.Response == true;
        await SetBusy(false);
    }
}