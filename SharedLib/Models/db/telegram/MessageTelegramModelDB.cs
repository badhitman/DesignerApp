////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// MessageTelegramModelDB
/// </summary>
[Index(nameof(MessageId), IsUnique = true)]
public class MessageTelegramModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Unique message identifier inside this chat
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public int MessageId { get; set; }

    /// <summary>
    /// Optional. Unique identifier of a message thread to which the message belongs; for supergroups only
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? MessageThreadId { get; set; }

    /// <summary>
    /// Optional. Sender, empty for messages sent to channels
    /// </summary>
    public UserTelegramModelDB? From { get; set; }

    /// <summary>
    /// Optional. Sender, empty for messages sent to channels
    /// </summary>
    public int? FromId { get; set; }

    /// <summary>
    /// Optional. Sender of the message, sent on behalf of a chat. The channel itself for channel messages.
    /// The supergroup itself for messages from anonymous group administrators. The linked channel for messages
    /// automatically forwarded to the discussion group
    /// </summary>
    public ChatTelegramModelDB? Chat { get; set; }

    /// <summary>
    /// Optional. Sender of the message, sent on behalf of a chat. The channel itself for channel messages.
    /// The supergroup itself for messages from anonymous group administrators. The linked channel for messages
    /// automatically forwarded to the discussion group
    /// </summary>
    public int ChatId { get; set; }

    /// <summary>
    /// Optional. Sender of the message, sent on behalf of a chat. The channel itself for channel messages.
    /// The supergroup itself for messages from anonymous group administrators. The linked channel for messages
    /// automatically forwarded to the discussion group
    /// </summary>
    public ChatTelegramModelDB? SenderChat { get; set; }
    /// <summary>
    /// Optional. Sender of the message, sent on behalf of a chat. The channel itself for channel messages.
    /// The supergroup itself for messages from anonymous group administrators. The linked channel for messages
    /// automatically forwarded to the discussion group
    /// </summary>
    public int? SenderChatId { get; set; }

    /// <summary>
    /// Optional. For forwarded messages, sender of the original message
    /// </summary>
    public int? ForwardFromId { get; set; }

    /// <summary>
    /// Optional. <see langword="true"/>, if the message is sent to a forum topic
    /// </summary>
    public bool? IsTopicMessage { get; set; }

    /// <summary>
    /// Optional. For messages forwarded from channels or from anonymous administrators, information about the
    /// original sender chat
    /// </summary>
    public int? ForwardFromChatId { get; set; }

    /// <summary>
    /// Optional. For messages forwarded from channels, identifier of the original message in the channel
    /// </summary>
    public int? ForwardFromMessageId { get; set; }

    /// <summary>
    /// Optional. For messages forwarded from channels, signature of the post author if present
    /// </summary>
    public string? ForwardSignature { get; set; }

    /// <summary>
    /// Optional. Sender's name for messages forwarded from users who disallow adding a link to their account in
    /// forwarded messages
    /// </summary>
    public string? ForwardSenderName { get; set; }

    /// <summary>
    /// Optional. For forwarded messages, date the original message was sent
    /// </summary>
    public DateTime? ForwardDate { get; set; }

    /// <summary>
    /// Optional. <see langword="true"/>, if the message is a channel post that was automatically forwarded to the connected
    /// discussion group
    /// </summary>
    public bool? IsAutomaticForward { get; set; }

    /// <summary>
    /// Optional. For replies, the original message. Note that the <see cref="MessageTelegramModelDB"/> object in this field
    /// will not contain further <see cref="ReplyToMessageId"/> fields even if it itself is a reply.
    /// </summary>
    public int? ReplyToMessageId { get; set; }

    /// <summary>
    /// Optional. Bot through which the message was sent <see cref="UserTelegramModelDB"/>
    /// </summary>
    public long? ViaBotUserId { get; set; }

    /// <summary>
    /// Optional. Date the message was last edited
    /// </summary>
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? EditDate { get; set; }

    /// <summary>
    /// CreatedAtUtc
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}