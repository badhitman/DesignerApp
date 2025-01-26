////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace SharedLib;

/// <summary>
/// UserTelegramModelDB
/// </summary>
[Index(nameof(UserTelegramId), IsUnique = true)]
[Index(nameof(Username)), Index(nameof(FirstName)), Index(nameof(LastName)), Index(nameof(IsBot))]
public class UserTelegramModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Unique identifier for this user or bot
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public long UserTelegramId { get; set; }

    /// <summary>
    /// <see langword="true"/>, if this user is a bot
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public bool IsBot { get; set; }


    /// <summary>
    /// User's or bot’s first name
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string FirstName { get; set; }
    /// <summary>
    /// User's or bot’s first name
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string NormalizedFirstNameUpper { get; set; }


    /// <summary>
    /// Optional. User's or bot’s last name
    /// </summary>
    public string? LastName { get; set; }
    /// <summary>
    /// Optional. User's or bot’s last name
    /// </summary>
    public string? NormalizedLastNameUpper { get; set; }


    /// <summary>
    /// Optional. User's or bot’s username
    /// </summary>
    public string? Username { get; set; }
    /// <summary>
    /// Optional. User's or bot’s username
    /// </summary>
    public string? NormalizedUsernameUpper { get; set; }


    /// <summary>
    /// Optional. <a href="https://en.wikipedia.org/wiki/IETF_language_tag">IETF language tag</a> of the
    /// user's language
    /// </summary>
    public string? LanguageCode { get; set; }

    /// <summary>
    /// Optional. <see langword="true"/>, if this user is a Telegram Premium user
    /// </summary>
    public bool? IsPremium { get; set; }

    /// <summary>
    /// Optional. <see langword="true"/>, if this user added the bot to the attachment menu
    /// </summary>
    public bool? AddedToAttachmentMenu { get; set; }

    /// <summary>
    /// LastMessageUtc
    /// </summary>
    public DateTime LastUpdateUtc { get; set; }

    /// <summary>
    /// LastMessageId
    /// </summary>
    public int LastMessageId { get; set; }

    /// <summary>
    /// Messages
    /// </summary>
    public List<MessageTelegramModelDB>? Messages { get; set; }

    /// <summary>
    /// ChatsJoins
    /// </summary>
    public List<JoinUserChatModelDB>? ChatsJoins { get; set; }
}