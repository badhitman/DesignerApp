////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Базовая DB модель объекта с поддержкой -> int:Id +string:Name +bool:IsDeleted AND UpdatedAt
/// </summary>
[Index(nameof(Name))]
public class EntrySwitchableUpdatedModel : EntrySwitchableModel
{
    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime LastAtUpdatedUTC { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAtUTC { get; set; } = DateTime.UtcNow;
}