////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// JoinUserChatModelDB
/// </summary>
public class JoinUserChatModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// User
    /// </summary>
    public UserTelegramModelDB? User {  get; set; }
    /// <summary>
    /// User
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Chat
    /// </summary>
    public ChatTelegramModelDB? Chat { get; set; }
    /// <summary>
    /// Chat
    /// </summary>
    public int ChatId { get; set; }
}