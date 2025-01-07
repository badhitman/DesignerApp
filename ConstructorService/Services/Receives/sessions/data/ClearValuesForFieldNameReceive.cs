////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Удалить значения (введённые в сессиях) по имени поля
/// </summary>
public class ClearValuesForFieldNameReceive(IConstructorService conService) : IResponseReceive<FormFieldOfSessionModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ClearValuesForFieldNameReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(FormFieldOfSessionModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.ClearValuesForFieldName(payload);
    }
}