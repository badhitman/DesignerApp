////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SetRoleFoeUserRequestModel
/// </summary>
public class SetRoleFoeUserRequestModel
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