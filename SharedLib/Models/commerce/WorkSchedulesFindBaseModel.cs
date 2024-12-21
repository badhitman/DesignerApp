////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// WorkSchedulesFindBaseModel
/// </summary>
public class WorkSchedulesFindBaseModel
{
    /// <summary>
    /// Start date
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// End date
    /// </summary>
    public DateOnly EndDate { get; set; }
}
