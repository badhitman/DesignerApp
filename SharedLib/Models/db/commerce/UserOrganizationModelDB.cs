////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// UserOrganizationModelDB
/// </summary>
[Index(nameof(OrganizationId), nameof(UserPersonIdentityId), IsUnique = true)]
public class UserOrganizationModelDB : PersonalEntrySwitchableUpdatedModel
{
    /// <summary>
    /// Organization
    /// </summary>
    public OrganizationModelDB? Organization { get; set; }

    /// <summary>
    /// Organization
    /// </summary>
    public int OrganizationId { get; set; }
}