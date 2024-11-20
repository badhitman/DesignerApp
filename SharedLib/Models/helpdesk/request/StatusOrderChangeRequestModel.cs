////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Status order change request
/// </summary>
public class StatusOrderChangeRequestModel : StatusChangeRequestModel
{
    /// <summary>
    /// VersionDocument
    /// </summary>
    public required Guid VersionDocument { get; set; }
}