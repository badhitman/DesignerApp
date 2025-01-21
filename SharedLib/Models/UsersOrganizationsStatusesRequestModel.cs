////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// UsersOrganizationsStatusesRequestModel
/// </summary>
public class UsersOrganizationsStatusesRequestModel : UniversalSelectRequestModel
{
    /// <summary>
    /// UsersOrganizationsFilter
    /// </summary>
    public UsersOrganizationsStatusesEnum[]? UsersOrganizationsStatusesFilter { get; set; }

    /// <summary>
    /// Организации (фильтр)
    /// </summary>
    public int[]? OrganizationsFilter {  get; set; }
}