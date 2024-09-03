////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// AttachmentForOrderModelDB
/// </summary>
public class AttachmentForOrderModelDB : EntryModel
{
    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public OrderDocumentModelDB? OrderDocument { get; set; }
    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public int OrderDocumentId { get; set; }

    /// <summary>
    /// FileName
    /// </summary>
    public required string FileName { get; set; }
}