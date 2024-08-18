////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Событие изменения в обращении
/// </summary>
public class PulseViewModel : PulseIssueLowModel
{
    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User Id (Identity)
    /// </summary>
    public required string AuthorUserIdentityId { get; set; }
}