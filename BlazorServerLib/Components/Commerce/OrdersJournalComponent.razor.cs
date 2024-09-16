﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OrdersJournalComponent
/// </summary>
public partial class OrdersJournalComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    AuthenticationStateProvider AuthRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// OrganizationFilter
    /// </summary>
    [Parameter]
    public int? OrganizationFilter { get; set; }

    /// <summary>
    /// GoodsFilter
    /// </summary>
    [Parameter]
    public int? GoodsFilter { get; set; }

    /// <summary>
    /// OfferFilter
    /// </summary>
    [Parameter]
    public int? OfferFilter { get; set; }

    /// <summary>
    /// AddressForOrganization
    /// </summary>
    [Parameter]
    public int AddressForOrganization { get; set; } = default!;


    UserInfoMainModel user = default!;


    string GetStatus(OrderDocumentModelDB doc)
    {
        return "не зарегистрирован";
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<OrderDocumentModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req = new()
        {
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
            Payload = new()
            {
                SenderActionUserId = user.UserId,
                Payload = new()
                {
                    IncludeExternalData = true,
                    OrganizationFilter = OrganizationFilter,
                    AddressForOrganizationFilter = AddressForOrganization,
                    GoodsFilter = GoodsFilter,
                    OfferFilter = OfferFilter,
                }
            }
        };

        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>> res = await CommerceRepo.OrdersSelect(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (!res.Success() || res.Response?.Response is null)
            return new TableData<OrderDocumentModelDB>() { TotalItems = 0, Items = [] };

        return new TableData<OrderDocumentModelDB>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        AuthenticationState state = await AuthRepo.GetAuthenticationStateAsync();
        IsBusyProgress = false;
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();
    }
}