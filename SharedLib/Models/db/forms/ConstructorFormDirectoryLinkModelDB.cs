using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// связь формы со списком/справочником
/// </summary>
[Index(nameof(Name)), Index(nameof(SortIndex))]
public class ConstructorFormDirectoryLinkModelDB : ConstructorFieldFormBaseLowModel
{
    /// <summary>
    /// Сортировка 
    /// </summary>
    public int SortIndex { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int DirectoryId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public ConstructorFormDirectoryModelDB? Directory { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ConstructorFormModelDB? Owner { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public void Update(ConstructorFormDirectoryLinkModelDB field)
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

        ConstructorFormDirectoryLinkModelDB other = (ConstructorFormDirectoryLinkModelDB)o;

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