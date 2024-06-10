using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Форма
/// </summary>
[Index(nameof(Name), IsUnique = true)]
public class ConstructorFormModelDB : ConstructorFormBaseModel
{
    /// <summary>
    /// Поля формы
    /// </summary>
    public List<ConstructorFieldFormModelDB>? Fields { get; set; }

    /// <summary>
    /// Получить числовые поля
    /// </summary>
    /// <param name="exclude_field_name">удалить из результата колонку</param>
    /// <returns>числовые поля</returns>
    public IQueryable<ConstructorFieldFormModelDB>? QueryFieldsOfNumericTypes(string? exclude_field_name) => Fields?.Where(x => (string.IsNullOrWhiteSpace(exclude_field_name) || !x.Name.Equals(exclude_field_name)) && ((new TypesFieldsFormsEnum[] { TypesFieldsFormsEnum.Double, TypesFieldsFormsEnum.Int, TypesFieldsFormsEnum.ProgrammCalcDouble }).Contains(x.TypeField))).OrderBy(x => x.SortIndex).AsQueryable();

    /// <summary>
    /// Связи форм со списками/связями
    /// </summary>
    public List<ConstructorFormDirectoryLinkModelDB>? FormsDirectoriesLinks { get; set; }

    /// <summary>
    /// Все поля формы (единым списком + отсортирован)
    /// </summary>
    [NotMapped]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ConstructorFieldFormBaseLowModel> AllFields
    {
        get
        {
            List<(ConstructorFieldFormBaseLowModel obj, int sort)> res = [];
            if (Fields is not null && Fields.Count != 0)
                foreach (ConstructorFieldFormModelDB f in Fields)
                {
                    f.Owner ??= this;
                    f.OwnerId = Id;
                    res.Add((f, f.SortIndex));
                }
            if (FormsDirectoriesLinks is not null && FormsDirectoriesLinks.Count != 0)
                foreach (ConstructorFormDirectoryLinkModelDB dl in FormsDirectoriesLinks)
                {
                    dl.Owner ??= this;
                    dl.OwnerId = Id;
                    res.Add((dl, dl.SortIndex));
                }

            return res
                .OrderBy(x => x.sort)
                .Select(x => x.obj)
                .ToArray();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static ConstructorFormModelDB Build(ConstructorFormModelDB other)
        => new()
        {
            Name = other.Name,
            Description = other.Description,
            Css = other.Css,
            AddRowButtonTitle = other.AddRowButtonTitle,
            Fields = other.Fields,
            FormsDirectoriesLinks = other.FormsDirectoriesLinks,
        };

    /// <summary>
    /// 
    /// </summary>
    public static ConstructorFormModelDB Build(ConstructorFormBaseModel other)
    {
        if (other is ConstructorFormModelDB form)
            return form;

        return new()
        {
            Name = other.Name,
            Description = other.Description,
            Css = other.Css,
            AddRowButtonTitle = other.AddRowButtonTitle
        };
    }

    /// <summary>
    /// Перезагрузка
    /// </summary>
    public ConstructorFormModelDB Reload(ConstructorFormModelDB other)
    {
        Name = other.Name;
        Description = other.Description;
        Css = other.Css;
        AddRowButtonTitle = other.AddRowButtonTitle;
        int i;
        if (other.Fields is not null)
        {
            Fields ??= [];
            i = Fields.FindIndex(x => !other.Fields.Any(y => y.Id == x.Id));
            while (i != -1)
            {
                Fields.RemoveAt(i);
                i = Fields.FindIndex(x => !other.Fields.Any(y => y.Id == x.Id));
            }
            ConstructorFieldFormModelDB? fo;
            foreach (ConstructorFieldFormModelDB f in Fields)
            {
                fo = other.Fields.FirstOrDefault(x => x.Id == f.Id);
                if (fo is not null)
                    f.Update(fo);
            }
            ConstructorFieldFormModelDB[] _fields = other.Fields.Where(x => !Fields.Any(y => y.Id == x.Id)).ToArray();
            if (_fields.Length != 0)
                Fields.AddRange(_fields);
        }

        if (other.FormsDirectoriesLinks is not null)
        {
            FormsDirectoriesLinks ??= [];

            i = FormsDirectoriesLinks.FindIndex(x => !other.FormsDirectoriesLinks.Any(y => y.Id == x.Id));
            while (i != -1)
            {
                FormsDirectoriesLinks.RemoveAt(i);
                i = FormsDirectoriesLinks.FindIndex(x => !other.FormsDirectoriesLinks.Any(y => y.Id == x.Id));
            }
            ConstructorFormDirectoryLinkModelDB? fo;
            foreach (ConstructorFormDirectoryLinkModelDB f in FormsDirectoriesLinks)
            {
                fo = other.FormsDirectoriesLinks.FirstOrDefault(x => x.Id == f.Id);
                if (fo is not null)
                    f.Update(fo);
            }
            ConstructorFormDirectoryLinkModelDB[] _fields = other.FormsDirectoriesLinks.Where(x => !FormsDirectoriesLinks.Any(y => y.Id == x.Id)).ToArray();
            if (_fields.Length != 0)
                FormsDirectoriesLinks.AddRange(_fields);
        }

        return this;
    }
}