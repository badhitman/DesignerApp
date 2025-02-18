////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Таб/вкладка документа
/// </summary>
[Index(nameof(Name), nameof(OwnerId), IsUnique = true)]
[Index(nameof(SortIndex), nameof(OwnerId), IsUnique = true)]
public class TabOfDocumentSchemeConstructorModelDB : EntryDescriptionOwnedModel
{
    /// <summary>
    /// Опрос/Анкета
    /// </summary>
    public DocumentSchemeConstructorModelDB? Owner { get; set; }

    /// <summary>
    /// Сортировка 
    /// </summary>
    public int SortIndex { get; set; }

    /// <summary>
    /// Связанные формы
    /// </summary>
    public List<FormToTabJoinConstructorModelDB>? JoinsForms { get; set; }

    /// <summary>
    /// Получить крайний элемент в границах restriction_sort_index
    /// </summary>
    public FormToTabJoinConstructorModelDB? GetOutermostJoinForm(DirectionsEnum direct, int restriction_sort_index)
    {
        if (JoinsForms is null || JoinsForms.Count == 0)
            return null;

        return direct == DirectionsEnum.Down
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
    public void Reload(TabOfDocumentSchemeConstructorModelDB other)
    {
        Name = other.Name;
        Description = other.Description;
    }

    /// <summary>
    /// Таб/вкладка документа
    /// </summary>
    public static TabOfDocumentSchemeConstructorModelDB Build(EntryDescriptionOwnedModel questionnaire_page, DocumentSchemeConstructorModelDB questionnaire_db, int sortIndex)
        => new()
        {
            Id = questionnaire_page.Id,
            Name = questionnaire_page.Name,
            Description = questionnaire_page.Description,
            OwnerId = questionnaire_page.OwnerId,
            Owner = questionnaire_db,
            SortIndex = sortIndex
        };

    /// <inheritdoc/>
    public static Dictionary<string, Dictionary<uint, List<ValueDataForSessionOfDocumentModelDB>>> GetRowsData(SessionOfDocumentDataModelDB session)
    {
        Dictionary<string, Dictionary<uint, List<ValueDataForSessionOfDocumentModelDB>>> res = [];
        if (session.DataSessionValues is null || session.DataSessionValues.Count == 0)
            return res;

        foreach (ValueDataForSessionOfDocumentModelDB val in session.DataSessionValues)
        {
            if (string.IsNullOrWhiteSpace(val.JoinFormToTab?.Tab?.Name))
                continue;

            if (!res.ContainsKey(val.JoinFormToTab.Tab.Name))
                res.Add(val.JoinFormToTab.Tab.Name, []);
            if (!res[val.JoinFormToTab.Tab.Name].ContainsKey(val.RowNum))
                res[val.JoinFormToTab.Tab.Name].Add(val.RowNum, []);

            res[val.JoinFormToTab.Tab.Name][val.RowNum].Add(val);
        }
        return res;
    }
}