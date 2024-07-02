////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Перечисление (лёгкая модель)
/// </summary>
public class EnumFitModel : EntryDescriptionModel
{
    /// <summary>
    /// Элементы/состав перечисления
    /// </summary>
    public IEnumerable<SortableFitModel>? EnumItems { get; set; }

    /// <inheritdoc/>
    public static explicit operator EnumFitModel(EnumDesignModelDB v)
    {
        return new EnumFitModel()
        {
            Description = v.Description,
            Id = v.Id,
            Name = v.Name,
            EnumItems = v.EnumItems?.Select(x => (SortableFitModel)x)
        };
    }
}
