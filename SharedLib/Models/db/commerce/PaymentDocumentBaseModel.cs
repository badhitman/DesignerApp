////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// PaymentDocumentBaseModel
/// </summary>
public class PaymentDocumentBaseModel : EntryModel
{
    /// <inheritdoc/>
    public override required string Name { get; set; }

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
    public int OrderDocumentId { get; set; }
}