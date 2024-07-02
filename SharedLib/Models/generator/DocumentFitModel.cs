////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Документ (лёгкая модель)
/// </summary>
public class DocumentFitModel : EntryDescriptionModel
{
    /// <summary>
    /// SystemName
    /// </summary>
    public required string SystemName { get; set; }

    /// <summary>
    /// Поля тела документа
    /// </summary>
    public IEnumerable<DocumentPropertyFitModel>? PropertiesBody { get; set; }

    /// <summary>
    /// Табличные части документа
    /// </summary>
    public IEnumerable<GridFitModel>? Grids { get; set; }
}