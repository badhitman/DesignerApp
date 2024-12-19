////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// UsersOrganizationsStatusesRequest
/// </summary>
public class UsersOrganizationsStatusesRequest : UniversalSelectRequestModel
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