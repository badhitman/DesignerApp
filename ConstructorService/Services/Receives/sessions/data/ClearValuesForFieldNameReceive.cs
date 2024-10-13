////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Удалить значения (введённые в сессиях) по имени поля
/// </summary>
public class ClearValuesForFieldNameReceive(IConstructorService conService)
    : IResponseReceive<FormFieldOfSessionModel?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ClearValuesForFieldNameReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(FormFieldOfSessionModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.ClearValuesForFieldName(payload);
        return new()
        {
            Messages = res.Messages,
        };
    }
}