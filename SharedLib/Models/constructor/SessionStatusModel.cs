////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SessionStatusModel
/// </summary>
public class SessionStatusModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public SessionsStatusesEnum Status { get; set; }
}