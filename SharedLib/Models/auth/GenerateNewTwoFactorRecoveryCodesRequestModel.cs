////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Генерация кодов восстановления для пользователя, что делает недействительными все предыдущие коды восстановления для пользователя.
/// </summary>
public class GenerateNewTwoFactorRecoveryCodesRequestModel
{
    /// <summary>
    /// Пользователь, для которого создаются коды восстановления.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Количество кодов, которые нужно сгенерировать.
    /// </summary>
    public byte Number {  get; set; }
}