////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components;

/// <summary>
/// LogsComponent
/// </summary>
public partial class LogsComponent
{
    [Inject]
    ILogsService LogsRepo { get; set; } = default!;


    bool _AllEventProperties;
    bool AllEventProperties
    {
        get => _AllEventProperties;
        set
        {
            _AllEventProperties = value;
            InvokeAsync(table.ReloadServerData);
        }
    }

    bool _ExceptionMessage;
    bool ExceptionMessage
    {
        get => _ExceptionMessage;
        set
        {
            _ExceptionMessage = value;
            InvokeAsync(table.ReloadServerData);
        }
    }

    bool _Logger;
    bool Logger
    {
        get => _Logger;
        set
        {
            _Logger = value;
            InvokeAsync(table.ReloadServerData);
        }
    }

    bool _CallSite;
    bool CallSite
    {
        get => _CallSite;
        set
        {
            _CallSite = value;
            InvokeAsync(table.ReloadServerData);
        }
    }

    bool _StackTrace;
    bool StackTrace
    {
        get => _StackTrace;
        set
        {
            _StackTrace = value;
            InvokeAsync(table.ReloadServerData);
        }
    }

    bool _ContextPrefix;
    bool ContextPrefix
    {
        get => _ContextPrefix;
        set
        {
            _ContextPrefix = value;
            InvokeAsync(table.ReloadServerData);
        }
    }

    private MudTable<NLogRecordModelDB> table = default!;

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
