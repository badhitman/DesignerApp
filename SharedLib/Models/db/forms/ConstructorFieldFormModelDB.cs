using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Поле формы
/// </summary>
[Index(nameof(SortIndex))]
public class ConstructorFieldFormModelDB : ConstructorFieldFormBaseModel
{
    /// <summary>
    /// Сортировка 
    /// </summary>
    public int SortIndex { get; set; }

    /// <inheritdoc/>
    public ConstructorFormModelDB? Owner { get; set; }



    /// <summary>
    /// Поле формы
    /// </summary>
    public static ConstructorFieldFormModelDB Build(ConstructorFieldFormBaseModel form_field, ConstructorFormModelDB form_db, int sortIndex)
        => new()
        {
            SystemName = form_field.SystemName,
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

    /// <inheritdoc/>
    public static ConstructorFieldFormModelDB Build(ConstructorFieldFormBaseLowModel form_field)
    {
        if (form_field is ConstructorFieldFormBaseModel bf)
        {
            return new ConstructorFieldFormModelDB()
            {
                SystemName = bf.SystemName,
                Name = bf.Name,
                Css = bf.Css,
                OwnerId = bf.OwnerId,
                Hint = bf.Hint,
                MetadataValueType = bf.MetadataValueType,
                Required = bf.Required,
                TypeField = bf.TypeField,
                Description = bf.Description,
                Id = bf.Id,
            };
        }
        else if (form_field is ConstructorFieldFormModelDB ff)
        {
            return new ConstructorFieldFormModelDB()
            {
                SystemName = ff.SystemName,
                Name = ff.Name,
                SortIndex = ff.SortIndex,
                Css = ff.Css,
                Description = ff.Description,
                Hint = ff.Hint,
                Id = ff.Id,
                MetadataValueType = ff.MetadataValueType,
                Owner = ff.Owner,
                OwnerId = ff.OwnerId,
                Required = ff.Required,
                TypeField = ff.TypeField,
            };
        }
        else
            throw new ArgumentException($"Тип поля не корректный: {form_field.GetType().FullName}", nameof(form_field));
    }


    /// <inheritdoc/>
    public override void Update(ConstructorFieldFormBaseLowModel form_field)
    {
        base.Update(form_field);

        if (form_field is ConstructorFieldFormModelDB _f)
        {
            MetadataValueType = _f.MetadataValueType;
            TypeField = _f.TypeField;

            Owner = _f.Owner;
            SortIndex = _f.SortIndex;
        }
        else if (form_field is ConstructorFieldFormBaseModel bf)
        {
            MetadataValueType = bf.MetadataValueType;
            TypeField = bf.TypeField;
        }
        else
            throw new ArgumentException($"Тип поля не корректный: {form_field.GetType().FullName}", nameof(form_field));
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
    public override int GetHashCode() => $"{Id} /{SystemName} {{{MetadataValueType}}}' [{Hint}] ({Description}) {TypeField} {Required} {OwnerId} '{Name}' `{Css}`".GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"#{Id}/{SystemName} '{Name}'/{Hint} [{Required}] ~{MetadataValueType}";
    }
}