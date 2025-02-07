////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Подбор записей (актуальных)
/// </summary>
public class RecordsAttendancesSelectReceive(ICommerceService commerceRepo)
    : IResponseReceive<TPaginationRequestAuthModel<RecordsAttendancesRequestModel>?, TPaginationResponseModel<RecordsAttendanceModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RecordsAttendancesSelectCommerceReceive;

    /// <summary>
    /// Подбор записей (актуальных)
    /// </summary>
    public async Task<TPaginationResponseModel<RecordsAttendanceModelDB>?> ResponseHandleAction(TPaginationRequestAuthModel<RecordsAttendancesRequestModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await commerceRepo.RecordsAttendancesSelect(payload);
    }
}