using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Элемент перечисления
/// </summary>
[Index(nameof(Name), nameof(ParentId), IsUnique = true)]
[Index(nameof(SortIndex))]
public class ConstructorFormDirectoryElementModelDB : EntryDescriptionModel
{
    /// <summary>
    /// Перечисление-владелец
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// Перечисление-владелец
    /// </summary>
    public ConstructorFormDirectoryModelDB? Parent { get; set; }

    /// <summary>
    /// Сортировка
    /// </summary>
    public int SortIndex { get; set; }
}