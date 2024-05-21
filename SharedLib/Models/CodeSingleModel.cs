using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Code
/// </summary>
public class CodeSingleModel
{
    /// <summary>
    /// Code
    /// </summary>
    [Required]
    [StringLength(7, ErrorMessage = "Длина {0} должна быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
    [DataType(DataType.Text)]
    [Display(Name = "Проверочный код")]
    public string Code { get; set; } = "";
}

/// <summary>
/// 
/// </summary>
public class LoginWithCodeModel
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    /// <summary>
    /// Пароль
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "Длина {0} должна быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    /// <summary>
    /// Повтор пароля
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Повтор пароля")]
    [Compare("Password", ErrorMessage = "Пароль и повтор пароля не совпадают.")]
    public string ConfirmPassword { get; set; } = "";

    /// <summary>
    /// Code
    /// </summary>
    [Required]
    public string Code { get; set; } = "";
}