////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Рубрики для обращений
/// </summary>
public class RubricIssueHelpdeskModelDB : EntryDescriptionModel
{
    /// <summary>
    /// Обращения в рубрике
    /// </summary>
    public List<IssueHelpdeskModelDB>? Issues { get; set; }

    /// <summary>
    /// Сортировка
    /// </summary>
    public uint SortIndex { get; set; }

    /// <summary>
    /// Владелец (вышестоящая рубрика)
    /// </summary>
    public RubricIssueHelpdeskModelDB? ParentRubric { get; set; }

    /// <summary>
    /// Владелец (вышестоящая рубрика)
    /// </summary>
    public int? ParentRubricId { get; set; }

    /// <summary>
    /// Вложенные рубрики
    /// </summary>
    public List<RubricIssueHelpdeskModelDB>? NestedRubrics { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }
}