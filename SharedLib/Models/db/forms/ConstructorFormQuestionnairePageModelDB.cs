using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Страница опроса/анкеты
/// </summary>
[Index(nameof(Name), nameof(OwnerId), IsUnique = true)]
[Index(nameof(SortIndex), nameof(OwnerId), IsUnique = true)]
public class ConstructorFormQuestionnairePageModelDB : EntryDescriptionOwnedModel
{
    /// <summary>
    /// Страница опроса/анкеты
    /// </summary>
    public static ConstructorFormQuestionnairePageModelDB Build(EntryDescriptionOwnedModel questionnaire_page, ConstructorFormQuestionnaireModelDB questionnaire_db, int sortIndex)
        => new()
        {
            Id = questionnaire_page.Id,
            Name = questionnaire_page.Name,
            Description = questionnaire_page.Description,
            OwnerId = questionnaire_page.OwnerId,
            Owner = questionnaire_db,
            SortIndex = sortIndex
        };

    /// <summary>
    /// Опрос/Анкета
    /// </summary>
    public ConstructorFormQuestionnaireModelDB? Owner { get; set; }

    /// <summary>
    /// Сортировка 
    /// </summary>
    public int SortIndex { get; set; }

    /// <summary>
    /// Связанные формы
    /// </summary>
    public List<ConstructorFormQuestionnairePageJoinFormModelDB>? JoinsForms { get; set; }

    /// <summary>
    /// Получить крайний элемент в границах restriction_sort_index
    /// </summary>
    public ConstructorFormQuestionnairePageJoinFormModelDB? GetOutermostJoinForm(VerticalDirectionsEnum direct, int restriction_sort_index)
    {
        if (JoinsForms is null || JoinsForms.Count == 0)
            return null;

        return direct == VerticalDirectionsEnum.Down
            ? JoinsForms.FirstOrDefault(x => x.SortIndex > restriction_sort_index)
            : JoinsForms.OrderByDescending(x => x.SortIndex).FirstOrDefault(x => x.SortIndex < restriction_sort_index);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"#{Id} '{Name}' ({nameof(SortIndex)}:{SortIndex})";
    }

    /// <summary>
    /// Перезагрузить
    /// </summary>
    public void Reload(ConstructorFormQuestionnairePageModelDB other)
    {
        Name = other.Name;
        Description = other.Description;
    }

    /// <summary>
    /// 
    /// </summary>
    public static Dictionary<string, Dictionary<uint, List<ConstructorFormSessionValueModelDB>>> GetRowsData(ConstructorFormSessionModelDB session)
    {
        Dictionary<string, Dictionary<uint, List<ConstructorFormSessionValueModelDB>>> res = [];
        if (session.SessionValues is null || session.SessionValues.Count == 0)
            return res;

        foreach (ConstructorFormSessionValueModelDB val in session.SessionValues)
        {
            if (string.IsNullOrWhiteSpace(val.QuestionnairePageJoinForm?.Owner?.Name))
                continue;

            if (!res.ContainsKey(val.QuestionnairePageJoinForm.Owner.Name))
                res.Add(val.QuestionnairePageJoinForm.Owner.Name, []);
            if (!res[val.QuestionnairePageJoinForm.Owner.Name].ContainsKey(val.GroupByRowNum))
                res[val.QuestionnairePageJoinForm.Owner.Name].Add(val.GroupByRowNum, []);

            res[val.QuestionnairePageJoinForm.Owner.Name][val.GroupByRowNum].Add(val);
        }
        return res;
    }
}