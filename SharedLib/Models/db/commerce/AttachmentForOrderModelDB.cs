////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Файл, прикреплённый к документу заказа
/// </summary>
public class AttachmentForOrderModelDB : EntryModel
{
    /// <inheritdoc/>
    public override required string Name { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public OrderDocumentModelDB? OrderDocument { get; set; }
    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public required int OrderDocumentId { get; set; }

    /// <summary>
    /// FilePoint
    /// </summary>
    public required string FilePoint { get; set; }

    /// <summary>
    /// FileSize
    /// </summary>
    public required long FileSize { get; set; }

    /// <summary>
    /// CreatedAtUTC
    /// </summary>
    public DateTime CreatedAtUTC { get; set; } = DateTime.UtcNow;
}