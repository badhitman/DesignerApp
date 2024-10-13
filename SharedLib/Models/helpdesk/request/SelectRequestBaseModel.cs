////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// SelectRequestBaseModel
/// </summary>
public class SelectRequestBaseModel
{
    /// <summary>
    /// IdentityUserId
    /// </summary>
    [Required]
    public required string[] IdentityUsersIds { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Строка поиска
    /// </summary>
    public string? SearchQuery { get; set; }
}