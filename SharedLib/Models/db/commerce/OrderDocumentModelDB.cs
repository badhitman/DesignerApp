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
    /// IdentityUserId
    /// </summary>
    public required string AuthorIdentityUserId { get; set; }

    /// <summary>
    /// Заявка, связанная с заказом.
    /// </summary>
    /// <remarks>
    /// До тех пор пока не указана заявка этот заказ всего лишь [Корзина]
    /// </remarks>
    public int? HelpdeskId { get; set; }

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