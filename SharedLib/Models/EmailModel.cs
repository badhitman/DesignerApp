////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Email
/// </summary>
public class EmailModel
{
    /// <summary>
    /// E-mail пользователя, доступ к которому требуется восстановить
    /// </summary>
    public string Email { get; set; } = string.Empty;
}