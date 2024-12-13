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
}