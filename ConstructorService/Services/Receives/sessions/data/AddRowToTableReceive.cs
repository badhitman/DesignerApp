////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Добавить новую строку в таблицу значений
/// </summary>
/// <returns>Номер п/п (начиная с 1) созданной строки</returns>
public class AddRowToTableReceive(IConstructorService conService)
    : IResponseReceive<FieldSessionDocumentDataBaseModel?, TResponseModel<int>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddRowToTableReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int>?> ResponseHandleAction(FieldSessionDocumentDataBaseModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.AddRowToTable(payload);
    }
}