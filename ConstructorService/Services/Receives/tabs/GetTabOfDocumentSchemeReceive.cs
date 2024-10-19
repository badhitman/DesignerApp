﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Получить страницу анкеты/опроса
/// </summary>
public class GetTabOfDocumentSchemeReceive(IConstructorService conService)
    : IResponseReceive<int?, TabOfDocumentSchemeConstructorModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetTabOfDocumentSchemeReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB?>> ResponseHandleAction(int? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<TabOfDocumentSchemeConstructorModelDB> res = await conService.GetTabOfDocumentScheme(payload.Value);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}