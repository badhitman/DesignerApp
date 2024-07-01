using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Phone
/// </summary>
public class PhoneSingleModel
{
    /// <summary>
    /// Phone
    /// </summary>
    [Phone]
    [Display(Name = "Номер телефона")]
    public string? PhoneNumber { get; set; }
}
