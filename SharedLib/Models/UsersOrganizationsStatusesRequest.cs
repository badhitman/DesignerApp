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
    public UsersOrganizationsStatusesEnum[]? UsersOrganizationsFilter { get; set; }
}