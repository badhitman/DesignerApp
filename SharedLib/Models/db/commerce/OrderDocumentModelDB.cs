////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Заказ (документ)
/// </summary>
public class OrderDocumentModelDB : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// Строки заказа
    /// </summary>
    public List<RowOfOrderDocumentModelDB>? Rows { get; set; }

    /// <summary>
    /// Deliveries
    /// </summary>
    public List<DeliveryModelDb>? Deliveries { get; set; }

    /// <summary>
    /// Вложения (файлы)
    /// </summary>
    public List<AttachmentForOrderModelDB>? Attachments { get; set; }
}