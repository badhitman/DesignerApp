////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// This object represents a phone contact.
/// </summary>
public class ContactTelegramModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Contact's phone number
    /// </summary>
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// Contact's first name
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Optional. Contact's last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Optional. Contact's user identifier in Telegram
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Optional. Additional data about the contact in the form of a vCard
    /// </summary>
    public string? Vcard { get; set; }

    /// <summary>
    /// Message
    /// </summary>
    public int MessageId { get; set; }
    /// <summary>
    /// Message
    /// </summary>
    public MessageTelegramModelDB? Message { get; set; }
}
