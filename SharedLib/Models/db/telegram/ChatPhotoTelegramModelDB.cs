////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// ChatPhotoTelegramModelDB
/// </summary>
public class ChatPhotoTelegramModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор файла небольшой фотографии чата (160x160).
    /// Этот FileId можно использовать только для загрузки фотографий и только до тех пор, пока фотография не будет изменена.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string SmallFileId { get; set; }

    /// <summary>
    /// Уникальный идентификатор файла небольшой (160x160) фотографии чата, который должен быть одинаковым во времени и для разных ботов.
    /// Невозможно использовать для загрузки или повторного использования файла.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string SmallFileUniqueId { get; set; }

    /// <summary>
    /// File identifier of big (640x640) chat photo. This FileId can be used only for photo download and only for
    /// as long as the photo is not changed.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string BigFileId { get; set; }

    /// <summary>
    /// Unique file identifier of big (640x640) chat photo, which is supposed to be the same over time and for
    /// different bots. Can't be used to download or reuse the file.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string BigFileUniqueId { get; set; }

    /// <summary>
    /// ChatOwner
    /// </summary>
    public ChatTelegramModelDB? ChatOwner { get; set; }

    /// <summary>
    /// ChatOwner
    /// </summary>
    public int ChatOwnerId { get; set; }
}