////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

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
    [Required(AllowEmptyStrings = false)]
    [Namespace]
    public required string Namespace { get; set; }
}