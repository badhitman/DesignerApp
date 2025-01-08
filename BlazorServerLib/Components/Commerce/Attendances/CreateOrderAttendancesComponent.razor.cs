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
public partial class CreateOrderAttendancesComponent : BlazorBusyComponentBaseAuthModel
{
    /// <summary>
    /// Commerce
    /// </summary>
    [Inject]
    protected ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    List<WorkScheduleModel>? Elements;
    List<WorkScheduleModel> _selectedSlots = [];

    void Closed(MudChip<WorkScheduleModel> chip)
    {
        lock (_selectedSlots)
        {
            int ind = _selectedSlots.FindIndex(x => x.Equals(chip.Value));
            if (ind > -1)
                _selectedSlots.RemoveAt(ind);
        }
    }

    void ToggleSelected(WorkScheduleModel sender)
    {
        lock (_selectedSlots)
        {
            int ind = _selectedSlots.FindIndex(x => x.Equals(sender));
            if (ind < 0)
                _selectedSlots.Add(sender);
            else
                _selectedSlots.RemoveAt(ind);
        }
    }

    DateRange _dateRange = new(DateTime.Now.Date, DateTime.Now.AddDays(5).Date);
    DateRange SelectedDateRange
    {
        get => _dateRange;
        set
        {
            _dateRange = value;
            InvokeAsync(ServerReload);
        }
    }

    TableGroupDefinition<WorkScheduleModel> _groupDefinition = new()
    {
        GroupName = "Дата",
        Indentation = false,
        Expandable = false,
        Selector = (e) => $"{e.Date} ({GlobalStaticConstants.RU.DateTimeFormat.GetDayName(e.Date.DayOfWeek)})"
    };

    async Task CreateOrder()
    {
        if (CurrentUserSession is null)
            throw new Exception("Пользователь не инициализирован");

        if (_selectedSlots is null)
            throw new Exception("Не выбраны слоты");

        if (SelectedOffer is null)
            throw new Exception("Offer не выбран");

        TAuthRequestModel<CreateAttendanceRequestModel> req = new()
        {
            SenderActionUserId = CurrentUserSession.UserId,
            Payload = new()
            {
                Offer = SelectedOffer,
                Records = _selectedSlots
            }
        };

        await SetBusy();
        ResponseBaseModel res = await CommerceRepo.CreateAttendanceRecords(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (res.Success())
            _selectedSlots.Clear();

        await ServerReload();
    }

    async Task ServerReload()
    {
        if (_dateRange.Start is null || _dateRange.End is null || _selectedOfferId is null)
            return;

        WorkFindRequestModel req = new()
        {
            StartDate = DateOnly.FromDateTime(_dateRange.Start.Value),
            EndDate = DateOnly.FromDateTime(_dateRange.End.Value),
            OffersFilter = [_selectedOfferId.Value],
            ContextName = GlobalStaticConstants.Routes.ATTENDANCES_CONTROLLER_NAME,
        };
        await SetBusy();
        WorksFindResponseModel res = await CommerceRepo.WorksSchedulesFind(req);
        Elements = [.. res.WorksSchedulesViews
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartPart)
            .ThenBy(x => x.Organization.Name)];

        await SetBusy(false);
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
            if (_selectedOfferId == value)
                return;

            _selectedOfferId = value;
            SelectedOffer = AllOffers.First(x => x.Id == value);
            InvokeAsync(ServerReload);
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
        await Task.WhenAll([LoadOffers(0), ReadCurrentUser()]);
        await ServerReload();
        SelectedOfferId = AllOffers.FirstOrDefault()?.Id;
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
        TResponseModel<TPaginationResponseModel<OfferModelDB>> res = await CommerceRepo.OffersSelect(new() { Payload = req, SenderActionUserId = CurrentUserSession!.UserId });
        await SetBusy(false);
        if (res.Response?.Response is not null && res.Response.Response.Count != 0)
        {
            AllOffers!.AddRange(res.Response.Response);
            if (AllOffers.Count < res.Response.TotalRowsCount)
                await LoadOffers(page_num + 1);
        }
    }
}