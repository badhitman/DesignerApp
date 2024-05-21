using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Код восстановления
/// </summary>
public class RecoveryCodeSingleModel
{
    /// <summary>
    /// Код восстановления
    /// </summary>
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Код восстановления")]
    public string RecoveryCode { get; set; } = "";
}