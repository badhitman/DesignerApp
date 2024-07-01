////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// SortableFitModel
/// </summary>
public class SortableFitModel : IdNameDescriptionSimpleModel
{
    /// <summary>
    /// Индекс сортировки
    /// </summary>
    public uint SortIndex { get; set; }
        
    /// <inheritdoc/>
    public static explicit operator SortableFitModel(EnumDesignItemModelDB v)
    {
        return new SortableFitModel()
        {
            Id = v.Id,
            Description = v.Description,
            Name = v.Name,
            SortIndex = v.SortIndex
        };
    }
}
