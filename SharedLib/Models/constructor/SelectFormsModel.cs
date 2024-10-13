////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SelectFormsModel
/// </summary>
public class SelectFormsModel
{
    /// <summary>
    /// Request
    /// </summary>
    public required SimplePaginationRequestModel Request { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    public int ProjectId { get; set; }
}
