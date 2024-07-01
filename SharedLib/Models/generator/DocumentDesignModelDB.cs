////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Документ
/// </summary>
public class DocumentDesignModelDB : MainTypeModel
{
    /// <summary>
    /// Поля документа (тело документа)
    /// </summary>
    public ICollection<DocumentPropertyMainBodyModelDB>? PropertiesBody { get; set; }

    /// <summary>
    /// Табличные части документа
    /// </summary>
    public ICollection<DocumentGridModelDB>? Grids { get; set; }
}