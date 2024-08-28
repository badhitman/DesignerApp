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
    /// File identifier of small (160x160) chat photo. This FileId can be used only for photo download and only
    /// for as long as the photo is not changed.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public string SmallFileId { get; set; } = default!;

    /// <summary>
    /// Unique file identifier of small (160x160) chat photo, which is supposed to be the same over time and for
    /// different bots. Can't be used to download or reuse the file.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public string SmallFileUniqueId { get; set; } = default!;

    /// <summary>
    /// File identifier of big (640x640) chat photo. This FileId can be used only for photo download and only for
    /// as long as the photo is not changed.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public string BigFileId { get; set; } = default!;

    /// <summary>
    /// Unique file identifier of big (640x640) chat photo, which is supposed to be the same over time and for
    /// different bots. Can't be used to download or reuse the file.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public string BigFileUniqueId { get; set; } = default!;

    /// <summary>
    /// ChatOwner
    /// </summary>
    public ChatTelegramModelDB? ChatOwner { get; set; }

    /// <summary>
    /// ChatOwner
    /// </summary>
    public int ChatOwnerId { get; set; }
}