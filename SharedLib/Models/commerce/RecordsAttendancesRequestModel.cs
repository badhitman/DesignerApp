////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// RecordsAttendancesRequestModel
/// </summary>
public class RecordsAttendancesRequestModel: DocumentsSelectRequestBaseModel
{
    /// <summary>
    /// ContextName
    /// </summary>
    public required string? ContextName { get; set; }
}