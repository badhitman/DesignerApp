////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// System Entry Description Owned
/// </summary>
[Index(nameof(TokenUniqueRoute), IsUnique = true)]
public abstract class SystemEntryDescriptionOwnedModel : EntryDescriptionOwnedModel
{
    /// <inheritdoc/>
    public required string SystemName { get; set; }

    /// <inheritdoc/>
    public required string TokenUniqueRoute { get; set; }
}