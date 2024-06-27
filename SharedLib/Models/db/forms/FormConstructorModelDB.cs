////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Форма
/// </summary>
[Index(nameof(Name), nameof(ProjectId), IsUnique = true)]
public class FormConstructorModelDB : ConstructorFormBaseModel
{
    /// <inheritdoc/>
    public static FormConstructorModelDB BuildEmpty(int projectId)
        => new() { SystemName = "", Name = "", ProjectId = projectId };

    /// <summary>
    /// Поля формы
    /// </summary>
    public List<FieldFormConstructorModelDB>? Fields { get; set; }

    /// <summary>
    /// Получить числовые поля
    /// </summary>
    /// <param name="exclude_field_name">удалить из результата колонку</param>
    /// <returns>числовые поля</returns>
    public IQueryable<FieldFormConstructorModelDB>? QueryFieldsOfNumericTypes(string? exclude_field_name) => Fields?.Where(x => (string.IsNullOrWhiteSpace(exclude_field_name) || !x.Name.Equals(exclude_field_name)) && ((new TypesFieldsFormsEnum[] { TypesFieldsFormsEnum.Double, TypesFieldsFormsEnum.Int, TypesFieldsFormsEnum.ProgramCalculationDouble }).Contains(x.TypeField))).OrderBy(x => x.SortIndex).AsQueryable();

    /// <summary>
    /// Связи форм со списками/связями
    /// </summary>
    public List<LinkDirectoryToFormConstructorModelDB>? FormsDirectoriesLinks { get; set; }

    /// <summary>
    /// Все поля формы (единым списком + сквозная сортировка)
    /// </summary>
    [NotMapped]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public ConstructorFieldFormBaseLowModel[] AllFields
    {
        get
        {
            List<(ConstructorFieldFormBaseLowModel obj, int sort)> res = [];
            if (Fields is not null && Fields.Count != 0)
                foreach (FieldFormConstructorModelDB f in Fields)
                {
                    f.Owner ??= this;
                    f.OwnerId = Id;
                    res.Add((f, f.SortIndex));
                }
            if (FormsDirectoriesLinks is not null && FormsDirectoriesLinks.Count != 0)
                foreach (LinkDirectoryToFormConstructorModelDB dl in FormsDirectoriesLinks)
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

    /// <inheritdoc/>
    public static FormConstructorModelDB Build(FormConstructorModelDB other)
        => new()
        {
            Name = other.Name,
            Description = other.Description,
            Css = other.Css,
            AddRowButtonTitle = other.AddRowButtonTitle,
            Fields = other.Fields,
            FormsDirectoriesLinks = other.FormsDirectoriesLinks,
            Id = other.Id,
            ProjectId = other.ProjectId,
            Project = other.Project,
            SystemName = other.SystemName,
        };

    /// <inheritdoc/>
    public static FormConstructorModelDB Build(ConstructorFormBaseModel other)
    {
        if (other is FormConstructorModelDB form)
            return Build(form);

        return new()
        {
            Id = other.Id,
            SystemName = other.SystemName,
            Name = other.Name,
            Description = other.Description,
            Css = other.Css,
            AddRowButtonTitle = other.AddRowButtonTitle,
            ProjectId = other.ProjectId,
            Project = other.Project,
        };
    }

    /// <summary>
    /// Перезагрузка данных объекта
    /// </summary>
    public FormConstructorModelDB Reload(FormConstructorModelDB other)
    {
        SystemName = other.SystemName;
        Id = other.Id;
        Name = other.Name;
        Description = other.Description;
        Css = other.Css;
        AddRowButtonTitle = other.AddRowButtonTitle;
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
            FieldFormConstructorModelDB? fo;
            foreach (FieldFormConstructorModelDB f in Fields)
            {
                fo = other.Fields.FirstOrDefault(x => x.Id == f.Id);
                if (fo is not null)
                    f.Update(fo);
            }
            FieldFormConstructorModelDB[] _fields = other.Fields.Where(x => !Fields.Any(y => y.Id == x.Id)).ToArray();
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
            LinkDirectoryToFormConstructorModelDB? fo;
            foreach (LinkDirectoryToFormConstructorModelDB f in FormsDirectoriesLinks)
            {
                fo = other.FormsDirectoriesLinks.FirstOrDefault(x => x.Id == f.Id);
                if (fo is not null)
                    f.Update(fo);
            }
            LinkDirectoryToFormConstructorModelDB[] _fields = other.FormsDirectoriesLinks.Where(x => !FormsDirectoriesLinks.Any(y => y.Id == x.Id)).ToArray();
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
        if (obj is FormConstructorModelDB other)
            return
                SystemName == other.SystemName &&
                Id == other.Id &&
                Name.Equals(other.Name) &&
                Description == other.Description &&
                Css == other.Css &&
                AddRowButtonTitle == other.AddRowButtonTitle &&
                ProjectId == other.ProjectId;

        if (obj is ConstructorFormBaseModel base_other)
            return
                SystemName == base_other.SystemName &&
                Id == base_other.Id &&
                Name.Equals(base_other.Name) &&
                Description == base_other.Description &&
                Css == base_other.Css &&
                AddRowButtonTitle == base_other.AddRowButtonTitle &&
                ProjectId == base_other.ProjectId;

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => $"{Id} /{SystemName} *{Name}* ({Description}) '{Css}' `{AddRowButtonTitle}` [{ProjectId}]".GetHashCode();
}