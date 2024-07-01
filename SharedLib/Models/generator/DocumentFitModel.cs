////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Документ (лёгкая модель)
/// </summary>
public class DocumentFitModel : RealTypeModel
{
    /// <summary>
    /// Поля тела документа
    /// </summary>
    public IEnumerable<DocumentPropertyFitModel>? PropertiesBody { get; set; }

    /// <summary>
    /// Табличные части документа
    /// </summary>
    public IEnumerable<GridFitModel>? Grids { get; set; }

    /// <inheritdoc/>
    public static explicit operator DocumentFitModel(DocumentDesignModelDB v)
    {
        return new DocumentFitModel()
        {
            Id = v.Id,
            Description = v.Description,
            Name = v.Name,
            SystemName = v.SystemName,
            PropertiesBody = v.PropertiesBody?.Select(x => (DocumentPropertyFitModel)x),
            Grids = v.Grids?.Select(x => (GridFitModel)x)
        };
    }
}