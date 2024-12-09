////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// IssueReadMarkerModelDB
/// </summary>
[Index(nameof(LastReadAt), nameof(UserIdentityId), IsUnique = true)]
public class IssueReadMarkerHelpdeskModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public int IssueId { get; set; }

    /// <summary>
    /// Issue
    /// </summary>
    public IssueHelpdeskModelDB? Issue { get; set; }

    /// <summary>
    /// LastReadAt
    /// </summary>
    public required DateTime LastReadAt { get; set; }

    /// <summary>
    /// User Id (Identity)
    /// </summary>
    public required string UserIdentityId { get; set; }
}