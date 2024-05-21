using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Новый пароль
/// </summary>
public sealed class SetNewPasswordModel
{
    /// <summary>
    /// Новый пароль
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "Длина {0} должна быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string? NewPassword { get; set; }

    /// <summary>
    /// Подтвердите новый пароль
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Подтвердите новый пароль")]
    [Compare("NewPassword", ErrorMessage = "Новый пароль и пароль подтверждения не совпадают.")]
    public string? ConfirmPassword { get; set; }
}