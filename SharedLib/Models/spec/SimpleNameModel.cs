////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
///Simple Name`d
/// </summary>
public class SimpleNameModel
{
    /// <summary>
    /// Имя объекта
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле наименования обязательно для заполнения")]
    public required string Name { get; set; }

    /// <inheritdoc/>
    public static SimpleNameModel BuildEmpty()
    {
        return new() { Name = "" };
    }
}