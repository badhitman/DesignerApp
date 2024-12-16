////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// WorkCalendarComponent
/// </summary>
public partial class WorkCalendarComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Commerce
    /// </summary>
    [Inject]
    protected ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// Offer
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required OfferModelDB? Offer { get; set; }


    int _selectedPage = 1;
    int selectedPage
    {
        get => _selectedPage;
        set
        {
            bool nu = value != _selectedPage;

            _selectedPage = value;
            if (nu)
                InvokeAsync(Reload);
        }
    }

    /// <summary>
    /// Количество элементов в начале и конце нумерации страниц.
    /// </summary>
    int boundaryCount = 1;
    /// <summary>
    /// Количество элементов в середине пагинации.
    /// </summary>
    int middleCount = 1;
    int countPages = 0;
    List<WorkScheduleCalendarModelDB> worksSchedulesCalendars = [];

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await Reload();
    }

    async void WorkCalendarAddDateAction()
    {
        _selectedPage = 0;
        await Reload();
    }

    /// <summary>
    /// Reload
    /// </summary>
    public async Task Reload()
    {
        TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel> req = new()
        {
            Payload = new()
            {
                OfferFilter = Offer?.Id,
                ActualOnly = true,
            },
            PageNum = _selectedPage - 1,
        };
        
        await SetBusy();
        TResponseModel<TPaginationResponseModel<WorkScheduleCalendarModelDB>> res = await CommerceRepo.WorkScheduleCalendarsSelect(req);

        if (res.Response is not null)
        {
            countPages = (int)Math.Ceiling((decimal)res.Response.TotalRowsCount / res.Response.PageSize);
            if (res.Response.Response is not null)
            {
                worksSchedulesCalendars = res.Response.Response;
            }
        }

        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }
}