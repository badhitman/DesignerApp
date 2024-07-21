////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// SortableFitModel
/// </summary>
public class SortableFitModel : BaseFitModel
{
    /// <summary>
    /// Индекс сортировки
    /// </summary>
    public required int SortIndex { get; set; }
}