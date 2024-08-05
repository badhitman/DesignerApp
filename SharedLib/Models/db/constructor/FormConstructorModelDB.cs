////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Форма
/// </summary>
[Index(nameof(Name), nameof(ProjectId), IsUnique = true)]
public class FormConstructorModelDB : FormBaseConstructorModel
{   
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
    public List<FieldFormAkaDirectoryConstructorModelDB>? FieldsDirectoriesLinks { get; set; }

    /// <summary>
    /// Все поля формы (единым списком + сквозная сортировка)
    /// </summary>
    [NotMapped]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public FieldFormBaseLowConstructorModel[] AllFields
    {
        get
        {
            List<(FieldFormBaseLowConstructorModel obj, int sort)> res = [];
            if (Fields is not null && Fields.Count != 0)
                foreach (FieldFormConstructorModelDB f in Fields)
                {
                    f.Owner ??= this;
                    f.OwnerId = Id;
                    res.Add((f, f.SortIndex));
                }
            if (FieldsDirectoriesLinks is not null && FieldsDirectoriesLinks.Count != 0)
                foreach (FieldFormAkaDirectoryConstructorModelDB dl in FieldsDirectoriesLinks)
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
    public static FormConstructorModelDB BuildEmpty(int projectId)
        => new() { Name = "", ProjectId = projectId };

    /// <inheritdoc/>
    public static FormConstructorModelDB Build(FormConstructorModelDB other)
        => new()
        {
            Name = other.Name,
            Description = other.Description,
            Css = other.Css,
            AddRowButtonTitle = other.AddRowButtonTitle,
            Fields = other.Fields,
            FieldsDirectoriesLinks = other.FieldsDirectoriesLinks,
            Id = other.Id,
            ProjectId = other.ProjectId,
            Project = other.Project,
        };

    /// <inheritdoc/>
    public static FormConstructorModelDB Build(FormBaseConstructorModel other)
    {
        if (other is FormConstructorModelDB form)
            return Build(form);

        return new()
        {
            Id = other.Id,
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

        if (other.FieldsDirectoriesLinks is not null)
        {
            FieldsDirectoriesLinks ??= [];

            int findDirectory_link_for_remove_action() => FieldsDirectoriesLinks.FindIndex(x => !other.FieldsDirectoriesLinks.Any(y => y.Id == x.Id));
            i = findDirectory_link_for_remove_action();
            while (i != -1)
            {
                FieldsDirectoriesLinks.RemoveAt(i);
                i = findDirectory_link_for_remove_action();
            }
            FieldFormAkaDirectoryConstructorModelDB? fo;
            foreach (FieldFormAkaDirectoryConstructorModelDB f in FieldsDirectoriesLinks)
            {
                fo = other.FieldsDirectoriesLinks.FirstOrDefault(x => x.Id == f.Id);
                if (fo is not null)
                    f.Update(fo);
            }
            FieldFormAkaDirectoryConstructorModelDB[] _fields = other.FieldsDirectoriesLinks.Where(x => !FieldsDirectoriesLinks.Any(y => y.Id == x.Id)).ToArray();
            if (_fields.Length != 0)
                FieldsDirectoriesLinks.AddRange(_fields);
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
                Id == other.Id &&
                Name.Equals(other.Name) &&
                Description == other.Description &&
                Css == other.Css &&
                AddRowButtonTitle == other.AddRowButtonTitle &&
                ProjectId == other.ProjectId;

        if (obj is FormBaseConstructorModel base_other)
            return
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
        => $"{Id} *{Name}* ({Description}) '{Css}' `{AddRowButtonTitle}` [{ProjectId}]".GetHashCode();
}