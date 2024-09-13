////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Платёжный документ
/// </summary>
public class PaymentDocumentBaseModel : EntryModel
{
    /// <summary>
    /// Название
    /// </summary>
    public override required string Name { get; set; }

    /// <summary>
    /// Сумма оплаты
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Идентификатор документа из внешней системы (например 1С)
    /// </summary>
    public required string ExternalDocumentId { get; set; }

    /// <summary>
    /// Документ-заказ
    /// </summary>
    public int OrderDocumentId { get; set; }
}