////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Поле формы (тип: справочник/список)
/// </summary>
[Index(nameof(Required))]
public class FieldFormBaseLowConstructorModel : EntryDescriptionOwnedModel
{
    /// <summary>
    /// Подсказка
    /// </summary>
    public string? Hint { get; set; }

    /// <summary>
    /// Обязательность для заполнения
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// CSS класс формы
    /// </summary>
    public string? Css { get; set; } = "col-12";

    /// <summary>
    /// Обновить
    /// </summary>
    public virtual void Update(FieldFormBaseLowConstructorModel field)
    {
        Id = field.Id;
        OwnerId = field.OwnerId;
        Name = field.Name;
        Description = field.Description;
        Hint = field.Hint;
        Required = field.Required;
        Css = field.Css;
        if (this is FieldFormConstructorModelDB _ft && field is FieldFormConstructorModelDB _fo)
        {
            _ft.SortIndex = _fo.SortIndex;
            //
            _ft.TypeField = _fo.TypeField;
            _ft.MetadataValueType = _fo.MetadataValueType;
        }
        else if (this is FieldFormAkaDirectoryConstructorModelDB _ft2 && field is FieldFormAkaDirectoryConstructorModelDB _fo2)
        {
            _ft2.SortIndex = _fo2.SortIndex;
            //
            _ft2.Directory = _fo2.Directory;
            _ft2.DirectoryId = _fo2.DirectoryId;
            _ft2.IsMultiSelect = _fo2.IsMultiSelect;
        }
    }

    /// <inheritdoc/>
    public override bool Equals(object? o)
    {
        if (o?.GetType() != GetType())
            return false;

        if (o is FieldFormConstructorModelDB sf)
            return sf.Equals((FieldFormConstructorModelDB)this);
        else if (o is FieldFormAkaDirectoryConstructorModelDB df)
            return df.Equals((FieldFormAkaDirectoryConstructorModelDB)this);

        FieldFormBaseLowConstructorModel other = (FieldFormBaseLowConstructorModel)o;
        return
            Id == other.Id &&
            Name == other.Name &&
            Hint == other.Hint &&
            Required == other.Required &&
            Css == other.Css &&
            OwnerId == other.OwnerId &&
            Description == other.Description;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (this is FieldFormConstructorModelDB sf)
            return sf.GetHashCode();
        else if (this is FieldFormAkaDirectoryConstructorModelDB df)
            return df.GetHashCode();
        else
            return $"{Id} '{Name}' [{Description}] -{Hint} `{Required}` {OwnerId} *{Css}*".GetHashCode();
    }
}