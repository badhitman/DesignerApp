////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Базовая DB модель объекта с поддержкой -> int:Id +string:Name
/// </summary>
[Index(nameof(Name))]
public class EntryModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Имя объекта
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле наименования обязательно для заполнения")]
    public virtual required string Name { get; set; }

    /// <inheritdoc/>
    public static EntryModel Build(string name) => new() { Name = name };
}