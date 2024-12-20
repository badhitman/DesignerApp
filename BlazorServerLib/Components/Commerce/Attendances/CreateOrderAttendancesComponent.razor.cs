////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// CreateOrderAttendancesComponent
/// </summary>
public partial class CreateOrderAttendancesComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Commerce
    /// </summary>
    [Inject]
    protected ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    private DateRange _dateRange = new(DateTime.Now.Date, DateTime.Now.AddDays(5).Date);
    private TableGroupDefinition<WorkSchedulesViewModel> _groupDefinition = new()
    {
        GroupName = "Date",
        Indentation = false,
        Expandable = false,
        Selector = (e) => e.Date
    };

    private async Task<TableData<WorkSchedulesViewModel>> ServerReload(TableState state, CancellationToken token)
    {
        if (_dateRange.Start is null || _dateRange.End is null || _selectedOfferId is null)
            return new TableData<WorkSchedulesViewModel>() { TotalItems = 0, Items = [] };

        TPaginationRequestModel<WorkSchedulesFindRequestModel> req = new()
        {
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
            Payload = new()
            {
                StartDate = DateOnly.FromDateTime(_dateRange.Start.Value),
                EndDate = DateOnly.FromDateTime(_dateRange.End.Value),
                OffersFilter = [_selectedOfferId.Value]
            }
        };
        await SetBusy(token: token);
        TResponseModel<TPaginationResponseModel<WorkSchedulesViewModel>> res = await CommerceRepo.WorkSchedulesFind(req);
        await SetBusy(false, token: token);
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (!res.Success() || res.Response?.Response is null)
            return new TableData<WorkSchedulesViewModel>() { TotalItems = 0, Items = [] };

        return new TableData<WorkSchedulesViewModel>() { TotalItems = res.Response.TotalRowsCount, Items = res.Response.Response };
    }

    OfferModelDB? SelectedOffer { get; set; }
    int? _selectedOfferId;
    /// <summary>
    /// SelectedOfferId
    /// </summary>
    public int? SelectedOfferId
    {
        get => _selectedOfferId;
        set
        {
            _selectedOfferId = value;
            SelectedOffer = AllOffers.First(x => x.Id == value);
        }
    }
    List<OfferModelDB> AllOffers { get; set; } = [];
    IGrouping<NomenclatureModelDB?, OfferModelDB>[] OffersNodes => AllOffers.GroupBy(x => x.Nomenclature).ToArray();

    static bool IsDateDisabledHandler(DateTime dt)
        => DateOnly.FromDateTime(dt) < DateOnly.FromDateTime(DateTime.Now);

    static string AdditionalDateClassesHandler(DateTime dt)
    {
        return dt.DayOfWeek == 0 ? "red-text text-accent-4" : "";
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await LoadOffers(0);
    }

    /// <summary>
    /// LoadOffers
    /// </summary>
    protected async Task LoadOffers(int page_num)
    {
        if (page_num == 0)
            AllOffers.Clear();

        TPaginationRequestModel<OffersSelectRequestModel> req = new()
        {
            PageNum = page_num,
            PageSize = 10,
            SortBy = nameof(OfferModelDB.Name),
            SortingDirection = VerticalDirectionsEnum.Up,
            Payload = new()
            {
                ContextName = GlobalStaticConstants.Routes.ATTENDANCES_CONTROLLER_NAME
            }
        };
        await SetBusy();
        TResponseModel<TPaginationResponseModel<OfferModelDB>> res = await CommerceRepo.OffersSelect(req);
        await SetBusy(false);
        if (res.Success() && res.Response?.Response is not null && res.Response.Response.Count != 0)
        {
            AllOffers!.AddRange(res.Response.Response);
            if (AllOffers.Count < res.Response.TotalRowsCount)
                await LoadOffers(page_num + 1);
        }
    }
}