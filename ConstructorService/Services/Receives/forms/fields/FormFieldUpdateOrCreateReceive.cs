////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Обновить/создать поле формы (простой тип)
/// </summary>
public class FormFieldUpdateOrCreateReceive(IConstructorService conService) : IResponseReceive<TAuthRequestModel<FieldFormConstructorModelDB>?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FormFieldUpdateOrCreateReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TAuthRequestModel<FieldFormConstructorModelDB>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.FormFieldUpdateOrCreate(payload);
    }
}
