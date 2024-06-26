////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using SharedLib;

namespace IdentityLib;

/// <summary>
/// ApplicationUser response
/// </summary>
public class ApplicationUserResponseModel : ResponseBaseModel
{
    /// <summary>
    /// ApplicationUser
    /// </summary>
    public ApplicationUser? ApplicationUser { get; set; }
}