////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Базовая DB модель объекта с поддержкой -> int:Id +string:Name +string:Tag +bool:IsDeleted
/// </summary>
public class EntryTagModel : EntryModel
{
    /// <summary>
    /// Tag/Признак объекта
    /// </summary>
    public string? Tag { get; set; }


    /// <inheritdoc/>
    public static EntryTagModel Build(string name, string tag) => new() { Name = name, Tag = tag };

    /// <inheritdoc/>
    public static EntryTagModel Build(int id, string name, string tag) => new() { Id = id, Name = name, Tag = tag };
}
