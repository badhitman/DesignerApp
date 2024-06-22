////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// System OwnedNamed
/// </summary>
public class SystemOwnedNameModel : SystemNameModel
{
    /// <summary>
    /// Владелец
    /// </summary>
    public int OwnerId { get; set; }

    /// <inheritdoc/>
    public static SystemOwnedNameModel BuildEmpty(int ownerId)
        => new() { Name = "", SystemName = "", OwnerId = ownerId };
}