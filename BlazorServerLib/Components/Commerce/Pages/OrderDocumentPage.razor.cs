////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Reflection.Metadata;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Pages;

/// <summary>
/// OrderDocumentPage
/// </summary>
public partial class OrderDocumentPage : BlazorBusyComponentBaseModel
{
    [Inject]
    NavigationManager NavRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider AuthRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Заказ
    /// </summary>
    [Parameter]
    public int? DocumentId { get; set; }


    UserInfoMainModel user = default!;

    private OrderDocumentModelDB[]? pagedData;

    List<OrganizationModelDB> Organizations { get; set; } = [];

    OrganizationModelDB? _currentOrganization;
    OrganizationModelDB? currentOrganization
    {
        get => _currentOrganization;
        set
        {
            _currentOrganization = value;
        }
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<OrderDocumentModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req = new()
        {
            Payload = new()
            {
                SenderActionUserId = user.UserId,
                Payload = new()
                {
                    IsCartFilter = true,
                    IncludeExternalData = false,
                }
            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };

        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>?> res = await CommerceRepo.OrdersSelect(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (!res.Success() || res.Response?.Response is null)
        {
            pagedData = null;
            return new TableData<OrderDocumentModelDB>() { TotalItems = 0, Items = [] };
        }

        if (DocumentId > 0 && res.Response.Response.Count == 1)
        {
            NavRepo.NavigateTo($"/order-document/{res.Response.Response[0].Id}");
            return new TableData<OrderDocumentModelDB>() { TotalItems = 0, Items = [] };
        }

        if (DocumentId == 0 && res.Response.Response.Count == 0)
        {
            OrderDocumentModelDB order = new()
            {
                AuthorIdentityUserId = user.UserId,
                Name = "Заказ",

            };

            IsBusyProgress = true;
            TResponseModel<int?> cart_new = await CommerceRepo.OrderUpdate(order);
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(cart_new.Messages);
            if (cart_new.Success() && cart_new.Response.HasValue)
                NavRepo.NavigateTo($"/order-document/{cart_new.Response}");

            return new TableData<OrderDocumentModelDB>() { TotalItems = 0, Items = [] };
        }

        pagedData = [.. res.Response.Response];
        return new TableData<OrderDocumentModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        DocumentId ??= 0;
        AuthenticationState state = await AuthRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();
        if (DocumentId > 0)
        {
            TPaginationRequestModel<OrganizationsSelectRequestModel> req = new()
            {
                Payload = new()
                {
                    ForUserIdentityId = user.UserId,
                },
                PageNum = 0,
                PageSize = int.MaxValue,
                SortBy = nameof(OrderDocumentModelDB.Name),
                SortingDirection = VerticalDirectionsEnum.Up,
            };
            IsBusyProgress = true;
            TResponseModel<TPaginationResponseModel<OrganizationModelDB>?> res = await CommerceRepo.OrganizationsSelect(req);
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(res.Messages);
        }
    }
}