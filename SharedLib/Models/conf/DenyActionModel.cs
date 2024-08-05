////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрет действий
/// </summary>
public class DenyActionModel
{
    /// <summary>
    /// Запрещено ли действие
    /// </summary>
    public bool IsDeny { get; set; } = false;

    /// <summary>
    /// Сообщение о причинах запрета
    /// </summary>
    public string Message { get; set; } = string.Empty;
}