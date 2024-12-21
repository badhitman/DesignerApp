////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// WorkSchedulesFindRequestModel
/// </summary>
public class WorkSchedulesFindRequestModel
{
    /// <summary>
    /// Start date
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// End date
    /// </summary>
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Имя контекста для разделения различных селекторов независимо друг от друга
    /// </summary>
    public string? ContextName { get; set; }

    /// <summary>
    /// Offers
    /// </summary>
    public int[]? OffersFilter { get; set; }
}