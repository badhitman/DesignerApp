////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// GoodsSelectRequestModel
/// </summary>
public class GoodsSelectRequestModel
{
    /// <summary>
    /// Дата обновления должна быть не меньше указанной
    /// </summary>
    public DateTime? AfterDateUpdate { get; set; }
}