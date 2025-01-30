////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace ToolsMauiApp.Components;

/// <summary>
/// LogsComponent
/// </summary>
public partial class LogsComponent
{
    [Inject]
    ILogsService LogsRepo { get; set; } = default!;


    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<NLogRecordModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        TPaginationRequestModel<LogsSelectRequestModel> req = new()
        {
            Payload = new()
            {

            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };

        TPaginationResponseModel<NLogRecordModelDB> selector = await LogsRepo.LogsSelect(req);
        return new TableData<NLogRecordModelDB>() { TotalItems = selector.TotalRowsCount, Items = selector.Response };
    }
}
