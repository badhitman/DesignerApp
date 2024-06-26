////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Схема документа
/// </summary>
[Index(nameof(Name), IsUnique = true)]
public class DocumentSchemeConstructorModelDB : EntryConstructedModel
{
    /// <inheritdoc/>
    public static DocumentSchemeConstructorModelDB BuildEmpty(int projectId)
        => new()
        {
            Name = "",
            SystemName = "",
            ProjectId = projectId,
        };

    /// <inheritdoc/>
    public static DocumentSchemeConstructorModelDB Build(EntryConstructedModel questionnaire, int projectId)
        => new()
        {
            Id = questionnaire.Id,
            SystemName = questionnaire.SystemName,
            Name = questionnaire.Name,
            Description = questionnaire.Description,
            Pages = [],
            ProjectId = projectId,
            Project = questionnaire.Project,
        };

    /// <summary>
    /// Страницы
    /// </summary>
    public List<TabOfDocumentSchemeConstructorModelDB>? Pages { get; set; }

    /// <summary>
    /// Получить крайнюю (последнюю или первую) страницу документа с учётом ограничения индекса сортировки
    /// </summary>
    /// <param name="direct">Направление определения (первая или последняя)</param>
    /// <param name="restriction_sort_index">Индекс сортировки от которого следует отталкиваться</param>
    public TabOfDocumentSchemeConstructorModelDB? GetOutermostPage(VerticalDirectionsEnum direct, int restriction_sort_index)
    {
        if (Pages is null || Pages.Count == 0)
            return null;

        return direct == VerticalDirectionsEnum.Down
           ? Pages.OrderBy(x => x.SortIndex).FirstOrDefault(x => x.SortIndex > restriction_sort_index)
           : Pages.OrderByDescending(x => x.SortIndex).FirstOrDefault(x => x.SortIndex < restriction_sort_index);
    }

    /// <summary>
    /// Перезагрузка
    /// </summary>
    public void Reload(DocumentSchemeConstructorModelDB other)
    {
        Name = other.Name;
        Description = other.Description;
        SystemName = other.SystemName;
        ProjectId = other.ProjectId;
        Project = other.Project;
        if (other.Pages is null)
            Pages = null;
        else
        {
            Pages ??= [];
            int i = Pages.FindIndex(x => !other.Pages.Any(y => y.Id == x.Id));
            while (i >= 0)
            {
                Pages.RemoveAt(i);
                i = Pages.FindIndex(x => !other.Pages.Any(y => y.Id == x.Id));
            }
            TabOfDocumentSchemeConstructorModelDB? _p;
            foreach (TabOfDocumentSchemeConstructorModelDB p in Pages)
            {
                _p = other.Pages.FirstOrDefault(x => x.Id == p.Id);
                if (_p is not null)
                    p.Reload(_p);
            }
            TabOfDocumentSchemeConstructorModelDB[] pages = other.Pages.Where(x => !Pages.Any(y => y.Id == x.Id)).ToArray();
            if (pages.Length != 0)
                Pages.AddRange(pages);
        }
    }
}