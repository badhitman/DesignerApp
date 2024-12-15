////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// WorkScheduleModelDB
/// </summary>
[Index(nameof(Weekday))]
public class WorkScheduleModelDB : WorkScheduleBaseModelDB
{
    /// <summary>
    /// День недели
    /// </summary>
    public required DayOfWeek Weekday { get; set; }

    /// <inheritdoc/>
    [Required(AllowEmptyStrings = true, ErrorMessage = "Поле наименования обязательно для заполнения")]
    public override required string Name { get => base.Name; set => base.Name = value; }
}