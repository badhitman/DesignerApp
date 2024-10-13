////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Обновить/создать поле формы (простой тип)
/// </summary>
public class FormFieldUpdateOrCreateReceive(IConstructorService conService)
: IResponseReceive<TAuthRequestModel<FieldFormBaseConstructorModel>?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FormFieldUpdateOrCreateReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(TAuthRequestModel<FieldFormBaseConstructorModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.FormFieldUpdateOrCreate(payload);
        return new()
        {
            Messages = res.Messages,
        };
    }
}
