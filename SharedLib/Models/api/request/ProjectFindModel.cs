////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// ProjectFindModel
/// </summary>
public class ProjectFindModel
{
    /// <summary>
    /// ProjectId
    /// </summary>
    public required int ProjectId { get; set; }

    /// <summary>
    /// QuerySearch
    /// </summary>
    public string? QuerySearch { get; set; }
}