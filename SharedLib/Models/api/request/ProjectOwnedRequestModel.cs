////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Project Owned Request
/// </summary>
public class ProjectOwnedRequestModel
{
    /// <summary>
    /// Project id
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Owner id
    /// </summary>
    public int? OwnerId { get; set; }
}
