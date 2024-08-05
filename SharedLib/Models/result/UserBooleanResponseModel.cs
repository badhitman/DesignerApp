////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// UserInfoModel + Boolean
/// </summary>
public class UserBooleanResponseModel : TResponseModel<bool?>
{
    /// <summary>
    /// User info
    /// </summary>
    public UserInfoModel? UserInfo { get; set; }
}