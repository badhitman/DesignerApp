////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace SharedLib;

/// <summary>
/// ChatTelegramModelDB
/// </summary>
[Index(nameof(ChatTelegramId), IsUnique = true)]
[Index(nameof(Type)), Index(nameof(Title)), Index(nameof(Username)), Index(nameof(FirstName)), Index(nameof(LastName)), Index(nameof(IsForum))]
public class ChatTelegramModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Unique identifier for this chat. This number may have more
    /// than 32 significant bits and some programming languages may have
    /// difficulty/silent defects in interpreting it. But it has
    /// at most 52 significant bits, so a signed 64-bit integer
    /// or double-precision float type are safe for storing this identifier.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public long ChatTelegramId { get; set; }

    /// <summary>
    /// Type of chat, can be either “private”, “group”, “supergroup” or “channel”
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public ChatsTypesTelegramEnum Type { get; set; }


    /// <summary>
    /// Optional. Title, for supergroups, channels and group chats
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// Optional. Title, for supergroups, channels and group chats
    /// </summary>
    public string? NormalizedTitleUpper { get; set; }


    /// <summary>
    /// Optional. Username, for private chats, supergroups and channels if available
    /// </summary>
    public string? Username { get; set; }
    /// <summary>
    /// Optional. Username, for private chats, supergroups and channels if available
    /// </summary>
    public string? NormalizedUsernameUpper { get; set; }


    /// <summary>
    /// Optional. First name of the other party in a private chat
    /// </summary>
    public string? FirstName { get; set; }
    /// <summary>
    /// Optional. First name of the other party in a private chat
    /// </summary>
    public string? NormalizedFirstNameUpper { get; set; }


    /// <summary>
    /// Optional. Last name of the other party in a private chat
    /// </summary>
    public string? LastName { get; set; }
    /// <summary>
    /// Optional. Last name of the other party in a private chat
    /// </summary>
    public string? NormalizedLastNameUpper { get; set; }


    /// <summary>
    /// Optional. <see langword="true"/>, if the supergroup chat is a forum (has topics enabled)
    /// </summary>
    public bool? IsForum { get; set; }

    /// <summary>
    /// LastUpdateUtc
    /// </summary>
    public DateTime LastUpdateUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// LastMessageId
    /// </summary>
    public int LastMessageId { get; set; }

    /// <summary>
    /// ChatPhoto
    /// </summary>
    public ChatPhotoTelegramModelDB? ChatPhoto { get; set; }

    /// <summary>
    /// Messages
    /// </summary>
    public List<MessageTelegramModelDB>? Messages { get; set; }

    /// <summary>
    /// ChatsJoins
    /// </summary>
    public List<JoinUserChatModelDB>? UsersJoins { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        string res = Id < 1 ? "" : $"[{Type.DescriptionInfo()}]";

        if (!string.IsNullOrWhiteSpace(Title))
            res += $" /{Title.Trim()}/";

        if (!string.IsNullOrWhiteSpace(Username))
            res += $" (@{Username.Trim()})";

        return $"{$"{FirstName} {LastName}".Trim()} {res}".Trim();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is ChatTelegramModelDB ct)
            return
                ct.Title == Title &&
                ct.LastName == LastName &&
                ct.ChatTelegramId == ChatTelegramId &&
                ct.FirstName == FirstName &&
                ct.Id == Id &&
                ct.Type == Type &&
                ct.Username == Username;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Id}{ChatTelegramId}{Type}{Title}{Username}{FirstName}{LastName}{IsForum}".GetHashCode();
    }
}