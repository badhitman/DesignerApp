////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib.Models;

/// <summary>
/// Перечисление (лёгкая модель)
/// </summary>
public class EnumFitModel
{
    /// <summary>
    /// SystemName
    /// </summary>
    public required string SystemName { get; set; }

    /// <summary>
    /// Имя объекта
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле наименования обязательно для заполнения")]
    public virtual required string Name { get; set; }

    /// <summary>
    /// Описание/примечание для объекта
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Элементы/состав перечисления
    /// </summary>
    public SortableFitModel[]? EnumItems { get; set; }
}