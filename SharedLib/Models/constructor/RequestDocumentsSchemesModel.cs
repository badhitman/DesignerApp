////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// RequestDocumentsSchemesModel
/// </summary>
public class RequestDocumentsSchemesModel
{
    /// <summary>
    /// RequestPayload
    /// </summary>
    public required SimplePaginationRequestModel RequestPayload { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    public required int ProjectId { get; set; }
}