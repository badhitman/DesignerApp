////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// CreateAttendanceRequestModel
/// </summary>
public class CreateAttendanceRequestModel
{
    /// <summary>
    /// Records
    /// </summary>
    public required List<WorkScheduleModel> Records { get; set; }

    /// <summary>
    /// Торговое предложение
    /// </summary>
    public required OfferModelDB Offer { get; set; }
}
