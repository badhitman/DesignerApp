////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// PaymentDocument
/// </summary>
public class PaymentDocumentModelDb : EntryModel
{
    /// <summary>
    /// Сумма оплаты
    /// </summary>
    public double Amount { get; set; }


    /// <summary>
    /// ExternalIdDocument
    /// </summary>
    public required string ExternalDocumentId { get; set; }

    /// <summary>
    /// OrderDocument
    /// </summary>
    public OrderDocumentModelDB? OrderDocument { get; set; }

    /// <summary>
    /// OrderDocument
    /// </summary>
    public int OrderDocumentId { get; set; }
}