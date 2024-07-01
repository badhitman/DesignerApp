////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Табличная часть документа
/// </summary>
public class GridFitModel : BaseFitRealTypeModel
{
    /// <summary>
    /// Поля табличной части документа
    /// </summary>
    public IEnumerable<DocumentPropertyFitModel>? Properties { get; set; }

    /// <inheritdoc/>
    public static explicit operator GridFitModel(DocumentGridModelDB v)
    {
        return new GridFitModel()
        {
            Id = v.Id,
            Name = v.Name,
            Description = v.Description,
            SystemName = v.SystemName,
            Properties = v.Properties?.Select(x => (DocumentPropertyFitModel)x)
        };
    }
}