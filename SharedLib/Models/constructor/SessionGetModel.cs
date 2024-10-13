////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SessionGetModel 
/// </summary>
public class SessionGetModel
{
    /// <summary>
    /// SessionId
    /// </summary>
    public required int SessionId { get; set; }

    /// <summary>
    /// IncludeExtra
    /// </summary>
    public bool IncludeExtra { get; set; } = true;
}
