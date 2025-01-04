////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.constructor;

/// <summary>
/// Удалить набор значений сессии опроса/анкеты по номеру строки [GroupByRowNum].
/// Если индекс ниже нуля - удаляются все значения для указанной JoinForm (полная очистка таблицы или очистка всех значений всех поллей стандартной формы)
/// </summary>
public class DeleteValuesFieldsByGroupSessionDocumentDataByRowNumReceive(IConstructorService conService)
    : IResponseReceive<ValueFieldSessionDocumentDataBaseModel, ResponseBaseModel>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.DeleteValuesFieldsByGroupSessionDocumentDataByRowNumReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel?> ResponseHandleAction(ValueFieldSessionDocumentDataBaseModel? payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return await conService.DeleteValuesFieldsByGroupSessionDocumentDataByRowNum(payload);
    }
}