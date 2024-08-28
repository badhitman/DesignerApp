////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Projected Request
/// </summary>
public class TProjectedRequestModel<T>
{
    /// <summary>
    /// Project id
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// T - Request
    /// </summary>
    public required T Request { get; set; }
}