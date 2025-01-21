////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Проверяет указанную двухфакторную аутентификацию <see cref="VerificationCode"/> на соответствие <see cref="UserId"/>
/// </summary>
public class VerifyTwoFactorTokenRequestModel
{
    /// <summary>
    /// Токен для проверки
    /// </summary>
    public required string VerificationCode { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }
}