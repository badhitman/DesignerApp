////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Подбор торговых предложений (поиск по параметрам)
/// </summary>
public class OffersSelectRequestModel
{
    /// <summary>
    /// Дата обновления должна быть не меньше указанной
    /// </summary>
    public DateTime? AfterDateUpdate { get; set; }

    /// <summary>
    /// Идентификатор Номенклатуры
    /// </summary>
    public int[]? NomenclatureFilter { get; set; }

    /// <summary>
    /// ContextName
    /// </summary>
    public required string? ContextName { get; set; }
}