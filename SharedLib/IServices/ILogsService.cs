////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ILogsService
/// </summary>
public interface ILogsService
{
    /// <summary>
    /// LogsSelect
    /// </summary>
    public Task<TPaginationResponseModel<NLogRecordModelDB>> LogsSelect(TPaginationRequestModel<LogsSelectRequestModel> req);

    /// <summary>
    /// MetadataLogs
    /// </summary>
    public Task<TResponseModel<LogsMetadataResponseModel>> MetadataLogs(PeriodDatesTimesModel req);
}