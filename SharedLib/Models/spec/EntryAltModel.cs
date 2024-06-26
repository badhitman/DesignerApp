////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <inheritdoc/>
public class EntryAltModel
{
    /// <inheritdoc/>
    public required string Id { get; set; }

    /// <inheritdoc/>
    public required string? Name { get; set; }

    /// <inheritdoc/>
    public void Update(EntryAltModel other)
    {
        Id = other.Id;
        Name = other.Name;
    }
}