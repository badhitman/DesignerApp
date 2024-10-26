////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// TagArticleSetModel
/// </summary>
public class TagSetModel : EntryModel
{
    /// <summary>
    /// Приложение
    /// </summary>
    public required string ApplicationName { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public required string PropertyName { get; set; }

    /// <summary>
    /// Префикс
    /// </summary>
    public string? PrefixPropertyName { get; set; }

    /// <summary>
    /// Set
    /// </summary>
    public bool Set { get; set; }
}