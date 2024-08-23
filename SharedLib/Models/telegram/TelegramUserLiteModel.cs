////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// TelegramUserBaseModelDb
/// </summary>
public class TelegramUserLiteModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Объект отключён
    /// </summary>
    public bool IsDisabled { get; set; } = false;

    /// <summary>
    ///  true, if this user is a bot
    /// </summary>
    public bool IsBot { get; set; }

    /// <summary>
    /// User's or bot’s first name
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Optional. User's or bot’s last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Username
    /// </summary>
    public string? Username { get; set; }
}