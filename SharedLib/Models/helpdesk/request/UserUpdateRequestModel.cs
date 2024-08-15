////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
///  Subscribe Update Request
/// </summary>
public class UserUpdateRequestModel
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
}