////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// UsersProjectModel
/// </summary>
public class UsersProjectModel
{
    /// <summary>
    /// ProjectId
    /// </summary>
    public required int ProjectId { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public required string[] UsersIds { get; set; }
}