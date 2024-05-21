////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Области изменения опций профиля пользователя
/// </summary>
public enum UserProfileAreasEnum
{
    /// <summary>
    /// Изменение пароля
    /// </summary>
    [Description("Изменение пароля")]
    PasswordChange,

    /// <summary>
    /// Уничтожение сессии
    /// </summary>
    [Description("Уничтожение сессии")]
    KillSession
}