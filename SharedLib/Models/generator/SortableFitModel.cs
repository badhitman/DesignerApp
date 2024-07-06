////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib.Models;

/// <summary>
/// SortableFitModel
/// </summary>
public class SortableFitModel
{
    /// <summary>
    /// Имя объекта
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле наименования обязательно для заполнения")]
    public virtual required string Name { get; set; }

    /// <summary>
    /// Индекс сортировки
    /// </summary>
    public int SortIndex { get; set; }

    /// <summary>
    /// Описание/примечание для объекта
    /// </summary>
    public string? Description { get; set; }

    /// <inheritdoc/>
    public static explicit operator SortableFitModel(EnumDesignItemModelDB v)
    {
        return new SortableFitModel()
        {
            Description = v.Description,
            Name = v.Name,
            SortIndex = v.SortIndex
        };
    }
}
