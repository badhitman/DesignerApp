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
    /// Имя файла
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Идентификатор файла, который используется для чтения данных из хранилища
    /// </summary>
    public required string FilePoint { get; set; }

    /// <summary>
    /// Размер файла
    /// </summary>
    public required long FileSize { get; set; }

    /// <summary>
    /// Заказ (документ)
    /// </summary>
    public required int OrderDocumentId { get; set; }
}