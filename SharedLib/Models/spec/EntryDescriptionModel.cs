////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
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
    public static bool operator ==(EntryDescriptionModel? e1, EntryDescriptionModel? e2)
        => (e1 is null && e2 is null) || (e1?.Id == e2?.Id && e1?.Name == e2?.Name && e1?.Description == e2?.Description);

    /// <inheritdoc/>
    public static bool operator !=(EntryDescriptionModel? e1, EntryDescriptionModel? e2)
        => (e1 is null && e2 is not null) || (e1 is not null && e2 is null) || e1?.Id != e2?.Id || e1?.Name != e2?.Name || e1?.Description != e2?.Description;

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