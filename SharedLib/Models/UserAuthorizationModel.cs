////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Объект пользователя для авторизация (создание сессии)
/// </summary>
public class UserAuthorizationModel : UserAuthorizationLiteModel
{
    /// <summary>
    /// Чекбокс: запомнить меня
    /// </summary>
    public bool RememberMe { get; set; }
}