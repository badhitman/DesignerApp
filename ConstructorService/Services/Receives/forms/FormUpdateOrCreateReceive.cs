////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Обновить/создать форму (имя, описание, `признак таблицы`)
/// </summary>
public class FormUpdateOrCreateReceive(IConstructorService conService)
   : IResponseReceive<TAuthRequestModel<FormBaseConstructorModel>?, FormConstructorModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FormUpdateOrCreateReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB?>> ResponseHandleAction(TAuthRequestModel<FormBaseConstructorModel>? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        TResponseModel<FormConstructorModelDB> res = await conService.FormUpdateOrCreate(payload);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}
