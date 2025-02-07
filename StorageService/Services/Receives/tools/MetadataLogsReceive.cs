////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// MetadataLogsReceive
/// </summary>
/// <param name="storeRepo"></param>
public class MetadataLogsReceive(ISerializeStorage storeRepo)
    : IResponseReceive<PeriodDatesTimesModel?, TResponseModel<LogsMetadataResponseModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MetadataLogsReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<LogsMetadataResponseModel>?> ResponseHandleAction(PeriodDatesTimesModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await storeRepo.MetadataLogs(payload);
    }
}