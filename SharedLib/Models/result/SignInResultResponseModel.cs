////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Результат авторизауии пользователя
/// </summary>
public class SignInResultResponseModel: IdentityResultResponseModel
{
    /// <summary>
    /// UserId
    /// </summary>
    public string? UserId { get; set; }
}