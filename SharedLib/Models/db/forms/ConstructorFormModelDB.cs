using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Форма
/// </summary>
[Index(nameof(Name), IsUnique = true)]
public class ConstructorFormModelDB : ConstructorFormBaseModel
{
    /// <inheritdoc/>
    public static ConstructorFormModelDB BuildEmpty(int projectId)
        => new() { Name = "", SystemName = "", ProjectId = projectId };

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
    /// Все поля формы (единым списком + сквозная сортировка)
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
            SystemName = other.SystemName,
            Id = other.Id,
            IsDisabled = other.IsDisabled,
            ProjectId = other.ProjectId,
            Project = other.Project,
        };

    /// <summary>
    /// 
    /// </summary>
    public static ConstructorFormModelDB Build(ConstructorFormBaseModel other)
    {
        if (other is ConstructorFormModelDB form)
            return Build(form);

        return new()
        {
            Name = other.Name,
            Description = other.Description,
            Css = other.Css,
            AddRowButtonTitle = other.AddRowButtonTitle,
            SystemName = other.SystemName,
            ProjectId = other.ProjectId,
            Project = other.Project,
            IsDisabled = other.IsDisabled,
            Id = other.Id,
        };
    }

    /// <summary>
    /// Перезагрузка данных объекта
    /// </summary>
    public ConstructorFormModelDB Reload(ConstructorFormModelDB other)
    {
        Name = other.Name;
        Description = other.Description;
        Css = other.Css;
        AddRowButtonTitle = other.AddRowButtonTitle;
        SystemName = other.SystemName;
        Id = other.Id;
        IsDisabled = other.IsDisabled;
        ProjectId = other.ProjectId;
        Project = other.Project;

        int i;
        if (other.Fields is null)
            Fields = null;
        else
        {
            Fields ??= [];
            int find_field_for_remove_action() => Fields.FindIndex(x => !other.Fields.Any(y => y.Id == x.Id));
            i = find_field_for_remove_action();
            while (i != -1)
            {
                Fields.RemoveAt(i);
                i = find_field_for_remove_action();
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

            int findDirectory_link_for_remove_action() => FormsDirectoriesLinks.FindIndex(x => !other.FormsDirectoriesLinks.Any(y => y.Id == x.Id));
            i = findDirectory_link_for_remove_action();
            while (i != -1)
            {
                FormsDirectoriesLinks.RemoveAt(i);
                i = findDirectory_link_for_remove_action();
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

    /// <summary>
    /// Сравнение с другой формой
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is ConstructorFormModelDB other)
            return
                Id == other.Id &&
                SystemName.Equals(other.SystemName) &&
                Name.Equals(other.Name) &&
                Description == other.Description &&
                Css == other.Css &&
                AddRowButtonTitle == other.AddRowButtonTitle &&
                IsDisabled == other.IsDisabled &&
                ProjectId == other.ProjectId;

        if (obj is ConstructorFormBaseModel base_other)
            return
                Id == base_other.Id &&
                SystemName.Equals(base_other.SystemName) &&
                Name.Equals(base_other.Name) &&
                Description == base_other.Description &&
                Css == base_other.Css &&
                AddRowButtonTitle == base_other.AddRowButtonTitle &&
                IsDisabled == base_other.IsDisabled &&
                ProjectId == base_other.ProjectId;

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => $"{Id} {SystemName} {Name} {Description} {Css} {AddRowButtonTitle} {IsDisabled} {ProjectId}".GetHashCode();
}