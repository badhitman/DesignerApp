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
            List<(ConstructorFieldFormBaseLowModel obj, int sort)> res = new();
            if (Fields?.Any() == true)
                foreach (ConstructorFieldFormModelDB f in Fields)
                {
                    f.Owner ??= this;
                    f.OwnerId = Id;
                    res.Add((f, f.SortIndex));
                }
            if (FormsDirectoriesLinks?.Any() == true)
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
}