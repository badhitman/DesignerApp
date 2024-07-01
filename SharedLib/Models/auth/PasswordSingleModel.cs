using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Пароль
/// </summary>
public class PasswordSingleModel
{
    /// <summary>
    /// Пароль
    /// </summary>
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}