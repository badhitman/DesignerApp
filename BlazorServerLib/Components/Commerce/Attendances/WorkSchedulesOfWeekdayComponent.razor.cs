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


    async void AddingWorkScheduleAction(WorkScheduleModelDB? sender)
    {
        await Reload();
    }

    async Task Reload()
    {
        TPaginationRequestModel<WorkSchedulesSelectRequestModel> req = new() 
        {
             Payload = new WorkSchedulesSelectRequestModel()
             {
                  //OfferFilter = 
             }
        };

        await SetBusy();
        TResponseModel<TPaginationResponseModel<WorkScheduleModelDB>> res = await CommerceRepo.WorkSchedulesSelect(req);
        await SetBusy(false);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await Reload();
    }
}