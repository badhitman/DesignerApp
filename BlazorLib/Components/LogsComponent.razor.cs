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
public partial class LogsComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ILogsService LogsRepo { get; set; } = default!;


    MudDateRangePicker _picker = default!;

    DateRange _dateRangeBind = new(DateTime.Now.Date, DateTime.Now.AddDays(5).Date);
    DateRange DateRangeBind
    {
        get => _dateRangeBind;
        set { _dateRangeBind = value; InvokeAsync(table.ReloadServerData); }
    }

    #region columns visible
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
    #endregion

    MudTable<NLogRecordModelDB> table = default!;

    async Task ReloadTable()
    {
        if (table is null)
            return;

        await SetBusy();
        await table.ReloadServerData();
        await SetBusy(false);
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<NLogRecordModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        await SetBusy(token: token);
        TPaginationRequestModel<LogsSelectRequestModel> req = new()
        {
            Payload = new()
            {
                StartAt = DateRangeBind.Start,
                FinalOff = DateRangeBind.End,
            },
            PageNum = state.Page,
            PageSize = state.PageSize,
            SortingDirection = state.SortDirection == SortDirection.Ascending ? VerticalDirectionsEnum.Up : VerticalDirectionsEnum.Down,
        };

        TPaginationResponseModel<NLogRecordModelDB> selector = await LogsRepo.LogsSelect(req);
        TResponseModel<LogsMetadataResponseModel> md = await LogsRepo.MetadataLogs(new() { StartAt = DateRangeBind.Start, FinalOff = DateRangeBind.End });


        await SetBusy(false, token);
        return new TableData<NLogRecordModelDB>() { TotalItems = selector.TotalRowsCount, Items = selector.Response };
    }
}