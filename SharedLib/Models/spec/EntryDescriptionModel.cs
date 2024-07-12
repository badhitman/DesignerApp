////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Базовая DB модель объекта с поддержкой -> int:Id +string:Name +string:Description
/// </summary>
public class EntryDescriptionModel : EntryModel
{
    /// <summary>
    /// Описание/примечание для объекта
    /// </summary>
    public string? Description { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj is EntryDescriptionModel e)
            return Name == e.Name && Description == e.Description && Id == e.Id;

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    => $"{Name} {Id} {Description}".GetHashCode();

    /// <inheritdoc/>
    public static new EntryDescriptionModel Build(string name)
        => new() { Name = name };

    /// <inheritdoc/>
    public static EntryDescriptionModel Build(string name, string description)
        => new() { Name = name, Description = description };
}