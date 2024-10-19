﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Сохранить данные формы документа из сессии
/// </summary>
public class SaveSessionFormReceive(IConstructorService conService)
    : IResponseReceive<SaveConstructorSessionRequestModel?, ValueDataForSessionOfDocumentModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SaveSessionFormReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<ValueDataForSessionOfDocumentModelDB[]?>> ResponseHandleAction(SaveConstructorSessionRequestModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<ValueDataForSessionOfDocumentModelDB[]> res = await conService.SaveSessionForm(payload);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}