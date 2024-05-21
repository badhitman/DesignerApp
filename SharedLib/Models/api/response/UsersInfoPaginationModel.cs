namespace SharedLib;

/// <summary>
/// Users info
/// </summary>
public class UsersInfoPaginationModel : PaginationResponseModel
{
    /// <summary>
    /// Users info
    /// </summary>
    public required List<UserInfoModel> UsersInfo { get; set; }
}