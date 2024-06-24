////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Базовая DB модель с поддержкой -> int:Id +bool:IsDeleted
/// </summary>
[Index(nameof(IsDisabled))]
public abstract class IdSwitchableModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Объект отключён
    /// </summary>
    public bool IsDisabled { get; set; } = false;
}
