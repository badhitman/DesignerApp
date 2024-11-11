////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Documents select request base
/// </summary>
public class DocumentsSelectRequestBaseModel
{
    /// <summary>
    /// Фильтр по номенклатуре
    /// </summary>
    public int? GoodsFilter { get; set; }

    /// <summary>
    /// Фильтр по коммерческому предложению
    /// </summary>
    public int? OfferFilter { get; set; }

    /// <summary>
    /// Загрузить дополнительные данные для документов
    /// </summary>
    public bool IncludeExternalData { get; set; }
}