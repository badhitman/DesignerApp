////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// StatusChangeRequestModel
/// </summary>
public class StatusChangeRequestModel
{
    /// <summary>
    /// Step
    /// </summary>
    public required StatusesDocumentsEnum Step { get; set; }

    /// <summary>
    /// Document Id
    /// </summary>
    public required int DocumentId { get; set; }
}