////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Документ
/// </summary>
public class DocumentFitModel : SortableFitModel
{
    /// <summary>
    /// Формы документа
    /// </summary>
    public required TabFitModel[] Tabs { get; set; }
}