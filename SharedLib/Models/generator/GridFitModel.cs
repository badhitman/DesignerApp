////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Табличная часть документа
/// </summary>
public class GridFitModel : EntryDescriptionModel
{
    /// <summary>
    /// Поля табличной части документа
    /// </summary>
    public IEnumerable<DocumentPropertyFitModel>? Properties { get; set; }

    /// <summary>
    /// SystemName
    /// </summary>
    public required string SystemName { get; set; }

    /// <inheritdoc/>
    public static explicit operator GridFitModel(DocumentGridModelDB v)
    {
        return new GridFitModel()
        {
            Id = v.Id,
            Name = v.Name,
            SystemName = v.SystemName,
            Description = v.Description,
            Properties = v.Properties?.Select(x => (DocumentPropertyFitModel)x)
        };
    }
}