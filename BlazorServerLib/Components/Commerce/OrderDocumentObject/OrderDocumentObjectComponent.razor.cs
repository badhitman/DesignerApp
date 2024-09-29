////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWebLib.Components.Commerce.OrderDocumentObject;

/// <summary>
/// OrderDocumentObjectComponent
/// </summary>
public partial class OrderDocumentObjectComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService StorageRepo { get; set; } = default!;

    [Inject]
    NavigationManager NavRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider AuthRepo { get; set; } = default!;


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

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await AuthRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();
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


        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<int> res = await StorageRepo.SaveParameter(doc, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId));
        
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
                Step = HelpdeskIssueStepsEnum.Canceled,
            }
        };
        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<bool> res = await HelpdeskRepo.StatusChange(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Response && res.Success())
            NavRepo.ReloadPage();
    }
}