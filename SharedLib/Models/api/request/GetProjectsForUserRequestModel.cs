////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// GetProjectsForUserRequestModel
/// </summary>
public class GetProjectsForUserRequestModel
{
    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// NameFilter
    /// </summary>
    public string? NameFilter { get; set; }
}