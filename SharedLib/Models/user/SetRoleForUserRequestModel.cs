////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SetRoleForUserRequestModel
/// </summary>
public class SetRoleForUserRequestModel
{
    /// <summary>
    /// UserIdentityId
    /// </summary>
    public required string UserIdentityId { get; set; }

    /// <summary>
    /// Имя роли
    /// </summary>
    public required string RoleName { get; set; }

    /// <summary>
    /// Удалить/Добавить (false/true)
    /// </summary>
    public bool Command { get; set; }
}