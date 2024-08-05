////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Установка нового пароля
/// </summary>
public class PasswordsPairModel
{
    /// <summary>
    /// Текущий пароль
    /// </summary>
    [Required(ErrorMessage = "Укажите текущий пароль")]
    public string PasswordCurrent { get; set; } = string.Empty;

    /// <summary>
    /// Новый пароль
    /// </summary>
    [StringLength(30, MinimumLength = 8, ErrorMessage = "Длина пароля должна быть от 8 до 30 символов")]
    [Unlike(nameof(PasswordCurrent), ErrorMessage = "Новый пароль совпадает со старым")]
    public string PasswordNew { get; set; } = string.Empty;

    /// <summary>
    /// Повтор нового пароля
    /// </summary>
    [Compare(nameof(PasswordNew), ErrorMessage = "Пароли не совпадают")]
    public string PasswordConfirm { get; set; } = string.Empty;
}