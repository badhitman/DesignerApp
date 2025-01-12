////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// Рабочие слоты для дня недели
/// </summary>
public partial class WorkSchedulesOfWeekdayComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceTransmission CommerceRepo { get; set; } = default!;


    /// <summary>
    /// Weekday
    /// </summary>
    [Parameter, EditorRequired]
    public required DayOfWeek Weekday { get; set; }

    /// <summary>
    /// Offer
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required OfferModelDB? Offer { get; set; }

    OfferModelDB? OfferCurrent;

    private readonly List<WeeklyScheduleModelDB> WorkSchedules = [];

    int TotalElementsCount;

    async void AddingWorkScheduleAction(WeeklyScheduleModelDB? sender)
    {
        await LoadData(0, OfferCurrent);
    }

    /// <summary>
    /// Reload
    /// </summary>
    public async Task LoadData(int pageNum, OfferModelDB? selectedOffer)
    {
        if (pageNum == 0)
            WorkSchedules.Clear();
        OfferCurrent = selectedOffer;

        await SetBusy();

        TPaginationRequestModel<WorkSchedulesSelectRequestModel> req = new()
        {
            Payload = new WorkSchedulesSelectRequestModel()
            {
                Weekdays = [Weekday],
                ContextName = GlobalStaticConstants.Routes.ATTENDANCES_CONTROLLER_NAME,
            },
            PageNum = pageNum
        };

        if (OfferCurrent is not null && OfferCurrent.Id != 0)
            req.Payload.OfferFilter = OfferCurrent.Id;

        TPaginationResponseModel<WeeklyScheduleModelDB> res = await CommerceRepo.WeeklySchedulesSelect(req);
        
        if (res.Response is not null)
        {
            TotalElementsCount = res.TotalRowsCount;
            WorkSchedules.AddRange(res.Response);
        }
        await SetBusy(false);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        OfferCurrent = Offer;
        await LoadData(0, OfferCurrent);
    }
}