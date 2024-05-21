using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Изменение пароля
/// </summary>
public class ChangePasswordModel
{
    /// <summary>
    /// Текущий пароль
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Текущий пароль")]
    public string OldPassword { get; set; } = "";

    /// <summary>
    /// Новый пароль
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "Длина {0} должна быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Новый пароль")]
    public string NewPassword { get; set; } = "";

    /// <summary>
    /// Подтверждение нового пароля
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Подтвердите новый пароль")]
    [Compare("NewPassword", ErrorMessage = "Новый пароль и пароль подтверждения не совпадают.")]
    public string ConfirmPassword { get; set; } = "";
}
