using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Email
/// </summary>
public class NewEmailSingleModel
{
    /// <summary>
    /// Email
    /// </summary>
    [Required]
    [EmailAddress]
    [Display(Name = "Новый E-mail")]
    public string? NewEmail { get; set; }
}