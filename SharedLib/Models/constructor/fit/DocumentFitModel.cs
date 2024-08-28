////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Документ
/// </summary>
public class DocumentFitModel : BaseFitModel
{
    /// <summary>
    /// Формы документа
    /// </summary>
    public required List<TabFitModel> Tabs { get; set; }
}