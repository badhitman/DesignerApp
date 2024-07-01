////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Сортируемая (лёгкая) вещественная модель
/// </summary>
public class SortableFitRealTypeModel : BaseFitRealTypeModel
{
    /// <summary>
    /// Индекс сортировки
    /// </summary>
    public uint SortIndex { get; set; }        
}
