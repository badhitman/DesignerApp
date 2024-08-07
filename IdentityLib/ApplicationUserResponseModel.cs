﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
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