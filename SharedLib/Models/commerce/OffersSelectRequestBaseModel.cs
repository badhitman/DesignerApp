////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Offers select request base
/// </summary>
public class OffersSelectRequestBaseModel
{
    /// <summary>
    /// Фильтр по номенклатуре
    /// </summary>
    public int? NomenclatureFilter { get; set; }

    /// <summary>
    /// Фильтр по коммерческому предложению
    /// </summary>
    public int? OfferFilter { get; set; }
}
