////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// WorkFindRequestModel
/// </summary>
public class WorkFindRequestModel : WorksFindBaseModel
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