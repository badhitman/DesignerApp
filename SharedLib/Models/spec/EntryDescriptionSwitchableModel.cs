////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Базовая DB модель объекта с поддержкой -> int:Id +string:Name +string:Description +bool:IsDeleted
/// </summary>
public class EntryDescriptionSwitchableModel : EntrySwitchableModel
{
    /// <inheritdoc/>
    public static new EntryDescriptionSwitchableModel Build(string name)
        => new() { Name = name };

    /// <inheritdoc/>
    public static EntryDescriptionSwitchableModel Build(string name, string description)
        => new() { Name = name, Description = description };

    /// <summary>
    /// Описание/примечание для объекта
    /// </summary>
    public string? Description { get; set; }
}