////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Удалить поле формы (тип: справочник/список)
/// </summary>
public class FormFieldDirectoryDeleteReceive(IConstructorService conService)
: IResponseReceive<TAuthRequestModel<int>?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FormFieldDirectoryDeleteReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(TAuthRequestModel<int>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.FormFieldDirectoryDelete(payload);
    }
}
