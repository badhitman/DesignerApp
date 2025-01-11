////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Создание нового пользователя (Identity)
/// </summary>
public class RegisterNewUserPasswordModel : UserAuthorizationLiteModel
{
    /// <summary>
    /// BaseAddress
    /// </summary>
    public required string BaseAddress { get; set; }
}