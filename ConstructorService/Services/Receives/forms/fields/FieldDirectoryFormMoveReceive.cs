////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Сдвинуть поле формы (тип: список/справочник)
/// </summary>
public class FieldDirectoryFormMoveReceive(IConstructorService conService) 
    : IResponseReceive<TAuthRequestModel<MoveObjectModel>, TResponseModel<FormConstructorModelDB>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FieldDirectoryFormMoveReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>?> ResponseHandleAction(TAuthRequestModel<MoveObjectModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.FieldDirectoryFormMove(payload);
    }
}
