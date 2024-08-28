////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Элемент перечисления
/// </summary>
[Index(nameof(SortIndex))]
[Index(nameof(Name), nameof(ParentId), IsUnique = true)]
public class ElementOfDirectoryConstructorModelDB : EntryDescriptionModel
{
    /// <summary>
    /// Перечисление-владелец
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// Перечисление-владелец
    /// </summary>
    public DirectoryConstructorModelDB? Parent { get; set; }

    /// <summary>
    /// Сортировка
    /// </summary>
    public required int SortIndex { get; set; }
}