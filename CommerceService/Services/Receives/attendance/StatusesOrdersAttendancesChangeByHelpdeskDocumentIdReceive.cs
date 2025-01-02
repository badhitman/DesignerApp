﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// StatusesOrdersAttendancesChangeByHelpdeskDocumentIdReceive
/// </summary>
public class StatusesOrdersAttendancesChangeByHelpdeskDocumentIdReceive(ICommerceService commRepo)
    : IResponseReceive<TAuthRequestModel<StatusChangeRequestModel>, TResponseModel<bool>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersAttendancesStatusesChangeByHelpdeskDocumentIdReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>?> ResponseHandleAction(TAuthRequestModel<StatusChangeRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commRepo.StatusesOrdersAttendancesChangeByHelpdeskDocumentId(req);
    }
}