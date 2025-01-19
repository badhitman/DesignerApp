////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Attendances;

/// <summary>
/// AttendancesCatalogComponent
/// </summary>
public partial class AttendancesCatalogComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ICommerceTransmission CommerceRepo { get; set; } = default!;


    bool _expanded;
    MudTable<NomenclatureModelDB> tableRef = default!;
    List<RecordsAttendanceModelDB> currentRecords = [];

    async void CreateNomenclatureAction(NomenclatureModelDB nom)
    {
        await tableRef.ReloadServerData();
        OnExpandCollapseClick();
        StateHasChanged();
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<NomenclatureModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        TPaginationRequestModel<NomenclaturesSelectRequestModel> req = new()
        {
            Payload = new() { ContextName = GlobalStaticConstants.Routes.ATTENDANCES_CONTROLLER_NAME },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortBy = state.SortLabel,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };
        await SetBusy(token: token);
        TPaginationResponseModel<NomenclatureModelDB> res = await CommerceRepo.NomenclaturesSelect(req);

        IsBusyProgress = false;

        if (res.Response is null)
            return new TableData<NomenclatureModelDB>() { TotalItems = 0, Items = [] };


        TPaginationRequestAuthModel<RecordsAttendancesRequestModel> recReq = new()
        {
            Payload = new RecordsAttendancesRequestModel()
            {
                NomenclatureFilter = [.. res.Response.Select(x => x.Id)],
                ContextName = GlobalStaticConstants.Routes.ATTENDANCES_CONTROLLER_NAME,
                IncludeExternalData = true,
            },
            SenderActionUserId = CurrentUserSession!.UserId,
            PageNum = 0,
            PageSize = int.MaxValue,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };

        TPaginationResponseModel<RecordsAttendanceModelDB> recordsSelect = await CommerceRepo.RecordsAttendancesSelect(recReq);
        List<RecordsAttendanceModelDB> currentRecords = recordsSelect.Response ?? [];

        return new TableData<NomenclatureModelDB>() { TotalItems = res.TotalRowsCount, Items = res.Response };
    }

    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }
}