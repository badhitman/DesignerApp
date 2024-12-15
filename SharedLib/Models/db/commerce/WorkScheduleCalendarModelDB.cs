////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// WorkScheduleCalendarModelDB
/// </summary>
[Index(nameof(DateScheduleCalendar))]
public class WorkScheduleCalendarModelDB : WorkScheduleBaseModelDB
{
    /// <summary>
    /// Дата
    /// </summary>
    public required DateOnly DateScheduleCalendar { get; set; }

    /// <inheritdoc/>
    [Required(AllowEmptyStrings = true, ErrorMessage = "Поле наименования обязательно для заполнения")]
    public override required string Name { get => base.Name; set => base.Name = value; }
}