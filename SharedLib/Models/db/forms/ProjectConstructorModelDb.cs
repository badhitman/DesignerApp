using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Project (for constructor)
/// </summary>
[Index(nameof(OwnerUserId))]
public class ProjectConstructorModelDb : EntryModel
{
    /// <summary>
    ///  Owner user (of Identity)
    /// </summary>
    public required string OwnerUserId { get; set; }

    /// <summary>
    /// Members
    /// </summary>
    public List<MemberOfProjectModelDb>? Members { get; set; }
}