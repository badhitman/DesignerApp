////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IssueThemeModelDB
/// </summary>
public class IssueThemeHelpdeskModelDB : EntryDescriptionModel
{
    /// <summary>
    /// Issues
    /// </summary>
    public List<IssueHelpdeskModelDB>? Issues { get; set; }
}