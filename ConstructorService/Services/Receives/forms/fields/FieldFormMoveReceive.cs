////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Сдвинуть поле формы (простой тип)
/// </summary>
public class FieldFormMoveReceive(IConstructorService conService)
    : IResponseReceive<TAuthRequestModel<MoveObjectModel>?, FormConstructorModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FieldFormMoveReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB?>> ResponseHandleAction(TAuthRequestModel<MoveObjectModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<FormConstructorModelDB> res = await conService.FieldFormMove(payload);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}
