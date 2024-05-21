////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Базовая DB модель объекта с поддержкой -> int:Id +string:Name +bool:IsDeleted
/// </summary>
public class EntryModel : IdSwitchableModel
{
    /// <inheritdoc/>
    public static EntryModel Build(string name) => new() { Name = name };

    /// <summary>
    /// Имя объекта
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле наименования обязательно для заполнения")]
    public string? Name { get; set; }
}