////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Documents select request base
/// </summary>
public class DocumentsSelectRequestBaseModel: OffersSelectRequestBaseModel
{
    /// <summary>
    /// SearchQuery
    /// </summary>
    public string? SearchQuery { get; set; }

    /// <summary>
    /// Загрузить дополнительные данные для документов
    /// </summary>
    public bool IncludeExternalData { get; set; }

    /// <summary>
    /// Дата обновления должна быть не меньше указанной
    /// </summary>
    public DateTime? AfterDateUpdate { get; set; }
}