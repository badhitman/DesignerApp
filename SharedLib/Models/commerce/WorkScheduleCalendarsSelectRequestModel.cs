////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// WorkScheduleCalendarsSelectRequestModel
/// </summary>
public class WorkScheduleCalendarsSelectRequestModel : DocumentsSelectRequestBaseModel
{
    /// <summary>
    /// DisabledOnly
    /// </summary>
    public bool? DisabledOnly { get; set; }

    /// <summary>
    /// ActualOnly
    /// </summary>
    public bool ActualOnly { get; set; }
}