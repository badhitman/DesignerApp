using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// 
/// </summary>
[Index(nameof(Name), nameof(ParentId), IsUnique = true)]
public class ConstructorFormDirectoryElementModelDB : EntryDescriptionModel
{
    /// <summary>
    /// 
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ConstructorFormDirectoryModelDB? Parent { get; set; }
}