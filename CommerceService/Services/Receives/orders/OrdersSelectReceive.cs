﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrdersSelectReceive
/// </summary>
public class OrdersSelectReceive(ICommerceService commRepo) 
    : IResponseReceive<TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>>?, TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>?> ResponseHandleAction(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commRepo.OrdersSelect(req);
    }
}