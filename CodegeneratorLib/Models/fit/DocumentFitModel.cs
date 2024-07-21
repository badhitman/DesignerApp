////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Документ
/// </summary>
public class DocumentFitModel : BaseFitModel
{
    /// <summary>
    /// Формы документа
    /// </summary>
    public required TabFitModel[] Tabs { get; set; }
}