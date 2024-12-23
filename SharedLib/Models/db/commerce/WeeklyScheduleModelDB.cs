////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// WeeklyScheduleModelDB
/// </summary>
[Index(nameof(Weekday))]
public class WeeklyScheduleModelDB : WorkScheduleBaseModelDB
{
    /// <summary>
    /// День недели
    /// </summary>
    public required DayOfWeek Weekday { get; set; }

    /// <inheritdoc/>
    [Required(AllowEmptyStrings = true, ErrorMessage = "Поле наименования обязательно для заполнения")]
    public override required string Name { get => base.Name; set => base.Name = value; }
}