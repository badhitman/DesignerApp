////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Связь пользователя с организацией
/// </summary>
[Index(nameof(OrganizationId), nameof(UserPersonIdentityId), IsUnique = true)]
public class UserOrganizationModelDB : PersonalEntrySwitchableUpdatedModel
{
    /// <summary>
    /// Организация
    /// </summary>
    public OrganizationModelDB? Organization { get; set; }

    /// <summary>
    /// Организация
    /// </summary>
    public int OrganizationId { get; set; }

    /// <summary>
    /// Статус
    /// </summary>
    public UsersOrganizationsStatusesEnum UserStatus { get; set; }
}