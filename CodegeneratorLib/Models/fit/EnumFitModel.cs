////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Перечисление (лёгкая модель)
/// </summary>
public class EnumFitModel : BaseFitModel
{
    /// <summary>
    /// Элементы/состав перечисления
    /// </summary>
    public required SortableFitModel[] EnumItems { get; set; }
}