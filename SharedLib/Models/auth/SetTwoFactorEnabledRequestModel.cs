////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Вкл/Выкл двухфакторную аутентификацию для указанного userId
/// </summary>
public class SetTwoFactorEnabledRequestModel
{
    /// <summary>
    /// EnabledSet
    /// </summary>
    public bool EnabledSet { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }
}