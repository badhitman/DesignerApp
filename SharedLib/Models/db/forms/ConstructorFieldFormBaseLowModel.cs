////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Поле формы (тип: справочник/список)
/// </summary>
[Index(nameof(Required))]
public class ConstructorFieldFormBaseLowModel : EntrySystemDescriptionOwnedModel
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
    public virtual void Update(ConstructorFieldFormBaseLowModel field)
    {
        SystemName = field.SystemName;
        Id = field.Id;
        OwnerId = field.OwnerId;
        Name = field.Name;
        Description = field.Description;
        Hint = field.Hint;
        Required = field.Required;
        Css = field.Css;
        if (this is ConstructorFieldFormModelDB _ft && field is ConstructorFieldFormModelDB _fo)
        {
            _ft.SortIndex = _fo.SortIndex;
            //
            _ft.TypeField = _fo.TypeField;
            _ft.MetadataValueType = _fo.MetadataValueType;
        }
        else if (this is ConstructorFormDirectoryLinkModelDB _ft2 && field is ConstructorFormDirectoryLinkModelDB _fo2)
        {
            _ft2.SortIndex = _fo2.SortIndex;
            //
            _ft2.Directory = _fo2.Directory;
            _ft2.DirectoryId = _fo2.DirectoryId;
        }
    }

    /// <inheritdoc/>
    public override bool Equals(object? o)
    {
        if (o?.GetType() != GetType())
            return false;

        if (o is ConstructorFieldFormModelDB sf)
            return sf.Equals((ConstructorFieldFormModelDB)this);
        else if (o is ConstructorFormDirectoryLinkModelDB df)
            return df.Equals((ConstructorFormDirectoryLinkModelDB)this);

        ConstructorFieldFormBaseLowModel other = (ConstructorFieldFormBaseLowModel)o;
        return
            Id == other.Id &&
            SystemName == other.SystemName &&
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
        if (this is ConstructorFieldFormModelDB sf)
            return sf.GetHashCode();
        else if (this is ConstructorFormDirectoryLinkModelDB df)
            return df.GetHashCode();
        else
            return $"{Id} /{SystemName} '{Name}' [{Description}] -{Hint} `{Required}` {OwnerId} *{Css}*".GetHashCode();
    }
}