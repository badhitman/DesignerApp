////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// OwnedNamed
/// </summary>
public class OwnedNameModel : SimpleNameModel
{
    /// <summary>
    /// Владелец
    /// </summary>
    public int OwnerId { get; set; }

    /// <inheritdoc/>
    public static OwnedNameModel BuildEmpty(int ownerId)
        => new() { Name = "", OwnerId = ownerId };
}