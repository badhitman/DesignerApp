////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Перечисления (enum)
/// </summary>
public class EnumDesignModelDB : MainTypeModel
{
    /// <summary>
    /// Состав/элементы перечисления
    /// </summary>
    public ICollection<EnumDesignItemModelDB>? EnumItems { get; set; }
}
