////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Таб/Вкладка документа
/// </summary>
public class TabFitModel : SortableFitModel
{
    /// <summary>
    /// Формы документа
    /// </summary>
    public required FormFitModel[] Forms { get; set; }
}