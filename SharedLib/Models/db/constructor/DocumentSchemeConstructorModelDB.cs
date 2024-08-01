////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Схема документа
/// </summary>
[Index(nameof(Name), nameof(ProjectId), IsUnique = true)]
public class DocumentSchemeConstructorModelDB : EntryConstructedModel
{
    /// <summary>
    /// Вкладки/Табы
    /// </summary>
    public List<TabOfDocumentSchemeConstructorModelDB>? Tabs { get; set; }

    /// <summary>
    /// Получить крайнюю (последнюю или первую) страницу документа с учётом ограничения индекса сортировки
    /// </summary>
    /// <param name="direct">Направление определения (первая или последняя)</param>
    /// <param name="restriction_sort_index">Индекс сортировки от которого следует отталкиваться</param>
    public TabOfDocumentSchemeConstructorModelDB? GetOutermostPage(VerticalDirectionsEnum direct, int restriction_sort_index)
    {
        if (Tabs is null || Tabs.Count == 0)
            return null;

        return direct == VerticalDirectionsEnum.Down
           ? Tabs.OrderBy(x => x.SortIndex).FirstOrDefault(x => x.SortIndex > restriction_sort_index)
           : Tabs.OrderByDescending(x => x.SortIndex).FirstOrDefault(x => x.SortIndex < restriction_sort_index);
    }

    /// <summary>
    /// Перезагрузка
    /// </summary>
    public void Reload(DocumentSchemeConstructorModelDB other)
    {
        Name = other.Name;
        Description = other.Description;
        ProjectId = other.ProjectId;
        Project = other.Project;
        if (other.Tabs is null)
            Tabs = null;
        else
        {
            Tabs ??= [];
            int i = Tabs.FindIndex(x => !other.Tabs.Any(y => y.Id == x.Id));
            while (i >= 0)
            {
                Tabs.RemoveAt(i);
                i = Tabs.FindIndex(x => !other.Tabs.Any(y => y.Id == x.Id));
            }
            TabOfDocumentSchemeConstructorModelDB? _p;
            foreach (TabOfDocumentSchemeConstructorModelDB p in Tabs)
            {
                _p = other.Tabs.FirstOrDefault(x => x.Id == p.Id);
                if (_p is not null)
                    p.Reload(_p);
            }
            TabOfDocumentSchemeConstructorModelDB[] pages = other.Tabs.Where(x => !Tabs.Any(y => y.Id == x.Id)).ToArray();
            if (pages.Length != 0)
                Tabs.AddRange(pages);
        }
    }

    /// <inheritdoc/>
    public static DocumentSchemeConstructorModelDB BuildEmpty(int projectId)
        => new()
        {
            Name = "",
            ProjectId = projectId,
        };

    /// <inheritdoc/>
    public static DocumentSchemeConstructorModelDB Build(EntryConstructedModel questionnaire, int projectId)
        => new()
        {
            Id = questionnaire.Id,
            Name = questionnaire.Name,
            Description = questionnaire.Description,
            Tabs = [],
            ProjectId = projectId,
            Project = questionnaire.Project,
        };
}