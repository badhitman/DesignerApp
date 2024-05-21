namespace SharedLib;

/// <summary>
/// Roles
/// </summary>
public class RolesPaginationModel : PaginationResponseModel
{
    /// <summary>
    /// Entries
    /// </summary>
    public required List<RoleInfoModel> Roles { get; set; }
}