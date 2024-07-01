using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Email
/// </summary>
public class EmailSingleModel
{
    /// <summary>
    /// Email
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";
}