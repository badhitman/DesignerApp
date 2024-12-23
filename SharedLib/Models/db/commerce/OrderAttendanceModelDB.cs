////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// OrderAttendancesModelDB
/// </summary>
public class OrderAttendanceModelDB : OrderDocumentBaseModelDB
{
    /// <summary>
    /// Rows
    /// </summary>
    public List<RowOfAttendanceModelDB>? Rows { get; set; }
}
