////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Базовая DB модель объекта с поддержкой -> int:Id +string:Name +string:Tag +bool:IsDeleted
/// </summary>
public class EntryTagModel : EntryModel
{
    /// <inheritdoc/>
    public static EntryTagModel Build(string name, string tag) => new() { Name = name, Tag = tag };

    /// <summary>
    /// Tag/Признак объекта
    /// </summary>
    public string? Tag { get; set; }
}
