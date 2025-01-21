namespace SharedLib;

/// <summary>
/// IdentitySetNewPasswordModel
/// </summary>
public class IdentityChangePasswordModel
{
    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Текущий пароль, который необходимо проверить перед изменением
    /// </summary>
    public required string CurrentPassword { get; set; }

    /// <summary>
    /// Новый пароль, который необходимо установить для указанного userId.Пользователь, пароль которого должен быть установлен
    /// </summary>
    public required string NewPassword { get; set; }
}