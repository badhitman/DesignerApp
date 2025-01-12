////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using System.Globalization;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// Журнал заказов
/// </summary>
public partial class OrdersJournalComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ICommerceTransmission CommerceRepo { get; set; } = default!;

    [Inject]
    IHelpdeskTransmission HelpdeskRepo { get; set; } = default!;


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
    public int? NomenclatureFilter { get; set; }

    /// <summary>
    /// Фильтр по торговому/коммерческому предложению
    /// </summary>
    [Parameter]
    public int? OfferFilter { get; set; }


    private static readonly CultureInfo cultureInfo = new("ru-RU");
    List<OrderDocumentModelDB> documentsPartData = [];
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
            SenderActionUserId = CurrentUserSession!.UserId,
            Payload = new()
            {
                IssuesIds = [.. q],
            }
        });
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Response is not null)
            IssuesCacheDump.AddRange(res.Response);
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
                SenderActionUserId = CurrentUserSession!.UserId,
                Payload = new()
                {
                    IncludeExternalData = true,
                    OrganizationFilter = OrganizationFilter,
                    AddressForOrganizationFilter = AddressForOrganization,
                    NomenclatureFilter = NomenclatureFilter,
                    OfferFilter = OfferFilter,
                }
            }
        };

        await SetBusy(token: token);
        
        TPaginationResponseModel<OrderDocumentModelDB> res = await CommerceRepo.OrdersSelect(req);
        IsBusyProgress = false;
        
        if (res.Response is null)
            return new TableData<OrderDocumentModelDB>() { TotalItems = 0, Items = [] };

        documentsPartData = res.Response;
        await UpdateCacheIssues();
        return new TableData<OrderDocumentModelDB>()
        {
            TotalItems = res.TotalRowsCount,
            Items = documentsPartData
        };
    }
}