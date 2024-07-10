////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// связь формы со списком/справочником
/// </summary>
[Index(nameof(SortIndex))]
public class FieldFormAkaDirectoryConstructorModelDB : FieldFormBaseLowConstructorModel
{
    /// <summary>
    /// Сортировка 
    /// </summary>
    public int SortIndex { get; set; }

    /// <summary>
    /// Справочник/список
    /// </summary>
    public int DirectoryId { get; set; }
    /// <summary>
    /// Справочник/список
    /// </summary>
    public DirectoryConstructorModelDB? Directory { get; set; }

    /// <summary>
    /// Форма
    /// </summary>
    public FormConstructorModelDB? Owner { get; set; }

    /// <summary>
    /// Update
    /// </summary>
    public void Update(FieldFormAkaDirectoryConstructorModelDB field)
    {
        base.Update(field);
        SortIndex = field.SortIndex;
        DirectoryId = field.DirectoryId;
        Directory = field.Directory;
    }

    /// <inheritdoc/>
    public override bool Equals(object? o)
    {
        if (o?.GetType() != GetType())
            return false;

        FieldFormAkaDirectoryConstructorModelDB other = (FieldFormAkaDirectoryConstructorModelDB)o;

        return
            other.Id == Id &&
            other.DirectoryId == DirectoryId &&
            other.Hint == Hint &&
            other.Description == Description &&
            other.Required == Required &&
            other.OwnerId == OwnerId &&
            other.Css == Css &&
            other.Name == Name;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => $"{Id}{DirectoryId}{Hint}{Description}{Required}{OwnerId}{Name}{Css}".GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"#{Id} '{Name}'/{Hint} [{Required}] ({OwnerId})";
    }
}