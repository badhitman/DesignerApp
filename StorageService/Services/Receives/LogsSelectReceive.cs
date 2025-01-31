////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// LogsSelectReceive
/// </summary>
public class LogsSelectReceive(ISerializeStorage storeRepo) 
    : IResponseReceive<TPaginationRequestModel<LogsSelectRequestModel>?, TPaginationResponseModel<NLogRecordModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.LogsSelectStorageReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NLogRecordModelDB>?> ResponseHandleAction(TPaginationRequestModel<LogsSelectRequestModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await storeRepo.LogsSelect(payload);
    }
}