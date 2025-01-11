////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// SimpleUserIdentityModel
/// </summary>
public class SimpleUserIdentityModel
{
    /// <summary>
    /// Логин пользователя для авторизации сессии
    /// </summary>
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// BaseAddress
    /// </summary>
    public required string BaseAddress { get; set; }
}