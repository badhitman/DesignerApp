﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Подобрать формы
/// </summary>
public class SelectFormsReceive(IConstructorService conService)
    : IResponseReceive<SelectFormsModel?, TPaginationResponseModel<FormConstructorModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SelectFormsReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<FormConstructorModelDB>?>> ResponseHandleAction(SelectFormsModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return new()
        {
            Response = await conService.SelectForms(payload),
        };
    }
}