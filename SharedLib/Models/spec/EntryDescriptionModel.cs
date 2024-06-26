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
    public static new EntryDescriptionModel Build(string name)
        => new() { Name = name };

    /// <inheritdoc/>
    public static EntryDescriptionModel Build(string name, string description)
        => new() { Name = name, Description = description };
}