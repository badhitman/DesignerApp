////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// TagModelDB
/// </summary>
[Index(nameof(NormalizedTagNameUpper))]
[Index(nameof(NormalizedTagNameUpper), nameof(OwnerPrimaryKey), IsUnique = true)]
public class TagModelDB : StorageBaseModelDB
{
    /// <summary>
    /// NormalizedTagNameUpper
    /// </summary>
    public string NormalizedTagNameUpper { get; set; } = default!;

    /// <summary>
    /// Имя параметра
    /// </summary>
    public required string TagName { get; set; }
}