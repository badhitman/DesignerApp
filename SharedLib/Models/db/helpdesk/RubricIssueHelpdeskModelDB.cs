////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IssueThemeModelDB
/// </summary>
public class RubricIssueHelpdeskModelDB : EntryDescriptionModel
{
    /// <summary>
    /// Issues
    /// </summary>
    public List<IssueHelpdeskModelDB>? Issues { get; set; }

    /// <summary>
    /// SortIndex
    /// </summary>
    public uint SortIndex { get; set; }

    /// <summary>
    /// ParentRubric
    /// </summary>
    public RubricIssueHelpdeskModelDB? ParentRubric { get; set; }

    /// <summary>
    /// ParentRubric
    /// </summary>
    public int? ParentRubricId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<RubricIssueHelpdeskModelDB>? SubRubrics { get; set; }
}