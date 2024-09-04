////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// AttachmentForOrderRequestModel
/// </summary>
public class AttachmentForOrderRequestModel
{
    /// <summary>
    /// FileName
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// FilePoint
    /// </summary>
    public required string FilePoint { get; set; }

    /// <summary>
    /// FileSize
    /// </summary>
    public required long FileSize { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public required int OrderDocumentId { get; set; }
}