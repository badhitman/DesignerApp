////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// This object represents a file ready to be downloaded. The file can be downloaded via
/// TelegramBotClientExtensions.GetFileAsync. It is guaranteed that the link will be valid for
/// at least 1 hour. When the link expires, a new one can be requested by calling
/// </summary>
public class FileBaseTelegramModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Identifier for this file, which can be used to download or reuse the file.
    /// Идентификатор этого файла, который можно использовать для загрузки или повторного использования файла.
    /// </summary>
    public required string FileId { get; set; }

    /// <summary>
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    /// Уникальный идентификатор файла, который должен быть одинаковым во времени и для разных ботов. Невозможно использовать для загрузки или повторного использования файла.
    /// </summary>
    public required string FileUniqueId { get; set; }

    /// <summary>
    /// Optional. File size
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// Message
    /// </summary>
    public int MessageId { get; set; }
    /// <summary>
    /// Message
    /// </summary>
    public MessageTelegramModelDB? Message { get; set; }
}