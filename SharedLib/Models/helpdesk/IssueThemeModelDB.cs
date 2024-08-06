////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IssueThemeModelDB
/// </summary>
public class IssueThemeModelDB : EntryDescriptionModel
{
    /// <summary>
    /// Issues
    /// </summary>
    public List<IssueModelDB>? Issues { get; set; }
}