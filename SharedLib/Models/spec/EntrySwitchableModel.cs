////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Базовая DB модель объекта с поддержкой -> int:Id +string:Name +bool:IsDeleted
/// </summary>
[Index(nameof(Name))]
public class EntrySwitchableModel : IdSwitchableModel
{
    /// <inheritdoc/>
    public static EntryModel Build(string name) => new() { Name = name };

    /// <summary>
    /// Имя объекта
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле наименования обязательно для заполнения")]
    public virtual required string Name { get; set; }
}