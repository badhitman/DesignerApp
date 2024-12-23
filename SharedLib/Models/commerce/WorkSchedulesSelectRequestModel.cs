////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// WorkSchedules select request
/// </summary>
public class WorkSchedulesSelectRequestModel : DocumentsSelectRequestBaseModel
{
    /// <summary>
    /// DisabledOnly
    /// </summary>
    public bool? DisabledOnly { get; set; }

    /// <summary>
    /// Weekday`s
    /// </summary>
    public DayOfWeek[]? Weekdays { get; set; }

    /// <summary>
    /// ContextName
    /// </summary>
    public required string ContextName { get; set; }
}