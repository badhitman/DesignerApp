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
    /// LastUpdatedAt
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }
}