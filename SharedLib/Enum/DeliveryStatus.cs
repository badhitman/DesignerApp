////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Статусы доставок
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// Создан
    /// </summary>
    Created,

    /// <summary>
    /// В пути
    /// </summary>
    Progress,

    /// <summary>
    /// Доставлен
    /// </summary>
    Done,
}