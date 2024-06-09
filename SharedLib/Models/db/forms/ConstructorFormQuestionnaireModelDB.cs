using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Опрос/Анкета
/// </summary>
[Index(nameof(Name), IsUnique = true)]
public class ConstructorFormQuestionnaireModelDB : EntryDescriptionModel
{
    /// <summary>
    /// Опрос/Анкета
    /// </summary>
    public static ConstructorFormQuestionnaireModelDB Build(EntryDescriptionModel questionnaire)
        => new()
        {
            Id = questionnaire.Id,
            Name = questionnaire.Name,
            Description = questionnaire.Description,
            Pages = []
        };

    /// <summary>
    /// Страницы
    /// </summary>
    public List<ConstructorFormQuestionnairePageModelDB>? Pages { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ConstructorFormQuestionnairePageModelDB? GetOutermostPage(VerticalDirectionsEnum direct, int restriction_sort_index)
    {
        if (Pages?.Any() != true)
            return null;

        return direct == VerticalDirectionsEnum.Down
           ? Pages.OrderBy(x => x.SortIndex).FirstOrDefault(x => x.SortIndex > restriction_sort_index)
           : Pages.OrderByDescending(x => x.SortIndex).FirstOrDefault(x => x.SortIndex < restriction_sort_index);
    }

    /// <summary>
    /// Перезагрузка
    /// </summary>
    public void Reload(ConstructorFormQuestionnaireModelDB other)
    {
        Name = other.Name;
        Description = other.Description;
        if (other.Pages is not null)
        {
            Pages ??= [];
            int i = Pages.FindIndex(x => !other.Pages.Any(y => y.Id == x.Id));
            while (i >= 0)
            {
                Pages.RemoveAt(i);
                i = Pages.FindIndex(x => !other.Pages.Any(y => y.Id == x.Id));
            }
            ConstructorFormQuestionnairePageModelDB? _p;
            foreach (ConstructorFormQuestionnairePageModelDB p in Pages)
            {
                _p = other.Pages.FirstOrDefault(x => x.Id == p.Id);
                if (_p is not null)
                    p.Reload(_p);
            }
            ConstructorFormQuestionnairePageModelDB[] pages = other.Pages.Where(x => !Pages.Any(y => y.Id == x.Id)).ToArray();
            if (pages.Length != 0)
                Pages.AddRange(pages);
        }
    }
}