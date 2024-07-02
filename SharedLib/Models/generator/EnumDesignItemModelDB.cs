////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib.Models;

/// <summary>
/// Элементы перечисления
/// </summary>
[Index(nameof(Name), nameof(OwnerEnumId), IsUnique = true)]
public class EnumDesignItemModelDB : EntryDescriptionModel
{
    /// <summary>
    /// Системное имя (имя типа/класса C#)
    /// </summary>
    [RegularExpression(GlobalStaticConstants.SYSTEM_NAME_TEMPLATE, ErrorMessage = GlobalStaticConstants.SYSTEM_NAME_TEMPLATE_MESSAGE)]
    public required override string Name { get; set; }

    /// <summary>
    /// Индекс сортировки
    /// </summary>
    public uint SortIndex { get; set; }

    /// <summary>
    /// Владелец элемента/перечисления
    /// </summary>
    public int OwnerEnumId { get; set; }

    /// <summary>
    /// Владелец элемента/перечисления
    /// </summary>
    public EnumDesignModelDB? OwnerEnum { get; set; }
}
