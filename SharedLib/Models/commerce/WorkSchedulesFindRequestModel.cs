////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// WorkSchedulesFindRequestModel
/// </summary>
public class WorkSchedulesFindRequestModel : WorkSchedulesFindBaseModel
{
    /// <summary>
    /// Имя контекста для разделения различных селекторов независимо друг от друга
    /// </summary>
    public string? ContextName { get; set; }

    /// <summary>
    /// Offers
    /// </summary>
    public required int[] OffersFilter { get; set; }
}