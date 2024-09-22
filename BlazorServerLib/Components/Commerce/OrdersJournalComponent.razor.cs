////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using System.Collections.Generic;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// Журнал заказов
/// </summary>
public partial class OrdersJournalComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider AuthRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Фильтр по организации
    /// </summary>
    [Parameter]
    public int? OrganizationFilter { get; set; }

    /// <summary>
    /// Фильтр по адресу организации
    /// </summary>
    [Parameter]
    public int AddressForOrganization { get; set; } = default!;

    /// <summary>
    /// Фильтр по номенклатуре
    /// </summary>
    [Parameter]
    public int? GoodsFilter { get; set; }

    /// <summary>
    /// Фильтр по торговому/коммерческому предложению
    /// </summary>
    [Parameter]
    public int? OfferFilter { get; set; }


    List<OrderDocumentModelDB> documentsPartData = [];
    UserInfoMainModel CurrentSessionUser = default!;
    readonly List<IssueHelpdeskModelDB> IssuesCacheDump = [];

    async Task UpdateCacheIssues()
    {
        IEnumerable<int> q = documentsPartData
            .Where(x => x.HelpdeskId.HasValue && x.HelpdeskId.Value > 0)
            .Select(x => x.HelpdeskId!.Value);

        if (!q.Any())
            return;

        TResponseModel<IssueHelpdeskModelDB[]> res = await HelpdeskRepo.IssuesRead(new()
        {
            SenderActionUserId = CurrentSessionUser.UserId,
            Payload = new()
            {
                IssuesIds = [.. q],
            }
        });
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        IssuesCacheDump.AddRange(res.Response!);
    }

    async Task<TableData<OrderDocumentModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req = new()
        {
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
            Payload = new()
            {
                SenderActionUserId = CurrentSessionUser.UserId,
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

        documentsPartData = res.Response.Response;
        await UpdateCacheIssues();
        return new TableData<OrderDocumentModelDB>()
        {
            TotalItems = res.Response.TotalRowsCount,
            Items = documentsPartData
        };
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        AuthenticationState state = await AuthRepo.GetAuthenticationStateAsync();
        IsBusyProgress = false;
        CurrentSessionUser = state.User.ReadCurrentUserInfo() ?? throw new Exception();
    }
}