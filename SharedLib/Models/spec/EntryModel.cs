////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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
    [NameValid]
    public virtual required string Name { get; set; }

    /// <inheritdoc/>
    public static EntryModel Build(string name) => new() { Name = name };

    /// <inheritdoc/>
    public static EntryModel Build(EntryModel sender) => new() { Name = sender.Name };

    /// <inheritdoc/>
    public static EntryModel BuildEmpty() => new() { Name = "" };

    /// <inheritdoc/>
    public void Update(EntryModel elementObjectEdit)
    {
        Name = elementObjectEdit.Name;
    }
}