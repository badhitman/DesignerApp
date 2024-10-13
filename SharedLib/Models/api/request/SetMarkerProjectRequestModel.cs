////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SetMarkerProjectRequestModel
/// </summary>
public class SetMarkerProjectRequestModel
{
    /// <summary>
    /// ProjectId
    /// </summary>
    public required int ProjectId { get; set; }

    /// <summary>
    /// Marker
    /// </summary>
    public bool Marker { get; set; }
}