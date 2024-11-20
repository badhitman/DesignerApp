////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Registers select request base
/// </summary>
public class RegistersSelectRequestBaseModel : OffersSelectRequestBaseModel
{
    /// <summary>
    /// Склад
    /// </summary>
    public required int WarehouseId { get; set; }
}
