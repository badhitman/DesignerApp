////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace ServerLib;

/// <summary>
/// LogsService
/// </summary>
public class LogsService(IStorageTransmission storeRepo) : ILogsService
{
    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NLogRecordModelDB>> LogsSelect(TPaginationRequestModel<LogsSelectRequestModel> req)
    {        
        return await storeRepo.LogsSelect(req);
    }
}