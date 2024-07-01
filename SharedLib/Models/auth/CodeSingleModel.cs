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