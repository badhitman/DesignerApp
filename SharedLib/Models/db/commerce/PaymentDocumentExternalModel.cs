////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// PaymentDocumentBaseModel
/// </summary>
public class PaymentDocumentExternalModel
{
    /// <summary>
    /// Имя объекта
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Сумма оплаты
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// Идентификатор документа из внешней системы
    /// </summary>
    public required string ExternalDocumentId { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public int OrderDocumentId { get; set; }
}