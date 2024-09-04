////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// PaymentDocument
/// </summary>
public class PaymentDocumentModelDb : PaymentDocumentBaseModel
{
    /// <summary>
    /// OrderDocument
    /// </summary>
    public OrderDocumentModelDB? OrderDocument { get; set; }
}