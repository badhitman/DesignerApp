////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// TagModelDB
/// </summary>
[Index(nameof(NormalizedNameUpper))]
[Index(nameof(NormalizedNameUpper), nameof(ContextName), IsUnique = true)]
public class TagModelDB : EntryModel
{
    /// <summary>
    /// ContextName
    /// </summary>
    public required string ContextName { get; set; }

    /// <summary>
    /// NormalizedNameUpper
    /// </summary>
    public string NormalizedNameUpper { get; set; } = default!;
}