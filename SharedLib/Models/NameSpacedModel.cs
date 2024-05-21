////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Имя и пространство имён
/// </summary>
public class NameSpacedModel
{
    /// <summary>
    /// Имя
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Пространство имён
    /// </summary>
    public required string NameSpace { get; set; }
}