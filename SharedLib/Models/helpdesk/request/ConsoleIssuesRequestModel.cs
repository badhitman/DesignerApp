////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ConsoleIssuesRequestModel
/// </summary>
public class ConsoleIssuesRequestModel
{
    /// <summary>
    /// Строка поиска
    /// </summary>
    public string? SearchQuery { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public required HelpdeskIssueStepsEnum Status { get; set; }
}