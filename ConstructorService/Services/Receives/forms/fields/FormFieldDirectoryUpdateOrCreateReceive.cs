////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Обновить/создать поле формы (тип: справочник/список)
/// </summary>
public class FormFieldDirectoryUpdateOrCreateReceive(IConstructorService conService)
: IResponseReceive<TAuthRequestModel<FieldFormAkaDirectoryConstructorModelDB>?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FormFieldDirectoryUpdateOrCreateReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(TAuthRequestModel<FieldFormAkaDirectoryConstructorModelDB>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.FormFieldDirectoryUpdateOrCreate(payload);
        return new()
        {
            Messages = res.Messages,
        };
    }
}
