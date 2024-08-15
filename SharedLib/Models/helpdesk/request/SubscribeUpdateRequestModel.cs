////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
///  Subscribe Update Request
/// </summary>
public class SubscribeUpdateRequestModel
{
    /// <summary>
    /// UserId
    /// </summary>
    [Required]
    public required string UserId { get; set; }

    /// <summary>
    /// Issue Id
    /// </summary>
    public required int IssueId { get; set; }

    /// <summary>
    /// Subscribe set
    /// </summary>
    public required bool SubscribeSet { get; set; }

    /// <summary>
    /// отключение отправки уведомлений
    /// </summary>
    public bool IsSilent { get; set; }
}
