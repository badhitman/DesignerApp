////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Registers select request base
/// </summary>
public class RegistersSelectRequestBaseModel
{
    /// <summary>
    /// Склад
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// Фильтр по номенклатуре
    /// </summary>
    public int[]? GoodsFilter { get; set; }

    /// <summary>
    /// Фильтр по коммерческому предложению
    /// </summary>
    public int[]? OfferFilter { get; set; }
}
