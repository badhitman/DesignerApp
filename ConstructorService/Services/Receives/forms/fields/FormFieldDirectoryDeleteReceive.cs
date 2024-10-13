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
: IResponseReceive<TAuthRequestModel<int>?, object?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FormFieldDirectoryDeleteReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<object?>> ResponseHandleAction(TAuthRequestModel<int>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ResponseBaseModel res = await conService.FormFieldDirectoryDelete(payload);
        return new()
        {
            Messages = res.Messages,
        };
    }
}
