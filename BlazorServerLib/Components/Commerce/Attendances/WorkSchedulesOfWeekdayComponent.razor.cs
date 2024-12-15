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
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


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


    List<WorkScheduleModelDB> WorkSchedules = [];

    int TotalElementsCount;

    async void AddingWorkScheduleAction(WorkScheduleModelDB? sender)
    {
        await LoadData(0);
    }

    /// <summary>
    /// Reload
    /// </summary>
    public async Task LoadData(int pageNum)
    {
        if (pageNum == 0)
            WorkSchedules.Clear();

        await SetBusy();

        TPaginationRequestModel<WorkSchedulesSelectRequestModel> req = new()
        {
            Payload = new WorkSchedulesSelectRequestModel()
            {
                OfferFilter = Offer?.Id,
                Weekdays = [Weekday]
            },
            PageNum = pageNum
        };
        TResponseModel<TPaginationResponseModel<WorkScheduleModelDB>> res = await CommerceRepo.WorkSchedulesSelect(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Response?.Response is not null)
        {
            TotalElementsCount = res.Response.TotalRowsCount;
            WorkSchedules.AddRange(res.Response.Response);
        }
        await SetBusy(false);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await LoadData(0);
    }
}