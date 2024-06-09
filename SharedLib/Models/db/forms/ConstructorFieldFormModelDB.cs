using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Поле формы
/// </summary>
[Index(nameof(Name)), Index(nameof(SortIndex))]
public class ConstructorFieldFormModelDB : ConstructorFieldFormBaseModel
{
    /// <summary>
    /// Поле формы
    /// </summary>
    public static ConstructorFieldFormModelDB Build(ConstructorFieldFormBaseModel form_field, ConstructorFormModelDB form_db, int sortIndex)
        => new()
        {
            Name = form_field.Name,
            Description = form_field.Description,
            Css = form_field.Css,
            OwnerId = form_field.OwnerId,
            Hint = form_field.Hint,
            MetadataValueType = form_field.MetadataValueType,
            Required = form_field.Required,
            TypeField = form_field.TypeField,
            Owner = form_db,
            SortIndex = sortIndex
        };

    /// <summary>
    /// Сортировка 
    /// </summary>
    public int SortIndex { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ConstructorFormModelDB? Owner { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public override void Update(ConstructorFieldFormBaseLowModel form_field)
    {
        base.Update(form_field);
        if (form_field is ConstructorFieldFormModelDB _f)
        {
            MetadataValueType = _f.MetadataValueType;
            TypeField = _f.TypeField;
        }
    }

    /// <inheritdoc/>
    public override bool Equals(object? o)
    {
        if (o?.GetType() != GetType())
            return false;

        ConstructorFieldFormModelDB other = (ConstructorFieldFormModelDB)o;

        bool res = other.Id == Id &&
            other.MetadataValueType == MetadataValueType &&
            other.Hint == Hint &&
            other.Css == Css &&
            other.Description == Description &&
            other.TypeField == TypeField &&
            other.Required == Required &&
            other.OwnerId == OwnerId &&
            other.Name == Name;

        return res;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => $"{Id}{MetadataValueType}{Hint}{Description}{TypeField}{Required}{OwnerId}{Name}{Css}".GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"#{Id} '{Name}'/{Hint} [{Required}] ~{MetadataValueType}";
    }
}