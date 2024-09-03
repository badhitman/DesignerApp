////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Базовая DB модель объекта с поддержкой -> int:Id +string:Name +DateTime:CreatedAt +bool:IsDeleted
/// </summary>
[Index(nameof(CreatedAt))]
public class EntryCreatedSwitchableModel : EntrySwitchableModel
{
    /// <inheritdoc/>
    public static new EntryCreatedSwitchableModel Build(string name) => new() { Name = name };

    /// <summary>
    /// Дата/время создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}