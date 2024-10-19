﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Перемещение страницы опроса/анкеты (сортировка страниц внутри опроса/анкеты)
/// </summary>
public class MoveTabOfDocumentSchemeReceive(IConstructorService conService)
    : IResponseReceive<TAuthRequestModel<MoveObjectModel>?, DocumentSchemeConstructorModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MoveTabOfDocumentSchemeReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB?>> ResponseHandleAction(TAuthRequestModel<MoveObjectModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<DocumentSchemeConstructorModelDB> res = await conService.MoveTabOfDocumentScheme(payload);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response
        };
    }
}