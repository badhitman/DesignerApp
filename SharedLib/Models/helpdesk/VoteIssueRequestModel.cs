////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Сообщение из обращения помечается как ответ
/// </summary>
public class VoteIssueRequestModel
{
    /// <summary>
    /// IdentityUserId
    /// </summary>
    [Required]
    public required string IdentityUserId { get; set; }

    /// <summary>
    /// MessageId
    /// </summary>
    public required int MessageId { get; set; }

    /// <summary>
    /// Признак: установить или удалить
    /// </summary>
    public required bool SetStatus { get; set; }
}