////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Объект (форма) регистрации пользователя
/// </summary>
public class UserRegistrationModel : UserAuthorizationLiteModel
{
    /// <summary>
    /// Повтор пароля пользователя
    /// </summary>
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтвердите пароль")]
    public string ConfirmPassword { get; set; } = string.Empty;
}