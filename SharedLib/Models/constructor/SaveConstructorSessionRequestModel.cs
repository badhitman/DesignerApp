////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SaveConstructorSessionRequestModel
/// </summary>
public class SaveConstructorSessionRequestModel
{
    /// <summary>
    /// SessionId
    /// </summary>
    public required int SessionId { get; set; }

    /// <summary>
    /// JoinFormToTab
    /// </summary>
    public required int JoinFormToTab { get; set; }

    /// <summary>
    /// SessionValues
    /// </summary>
    public required IEnumerable<ValueDataForSessionOfDocumentModelDB> SessionValues { get; set; }
}