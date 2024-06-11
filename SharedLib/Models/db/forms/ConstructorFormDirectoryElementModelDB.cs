using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Элемент перечисления
/// </summary>
[Index(nameof(Name), nameof(ParentId), IsUnique = true)]
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
}