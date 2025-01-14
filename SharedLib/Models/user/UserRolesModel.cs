////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// UserRolesModel
/// </summary>
public class UserRolesModel
{
    /// <summary>
    /// User Id (of Identity)
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Имена ролей
    /// </summary>
    public required List<string> RolesNames { get; set; }
}