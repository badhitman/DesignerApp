////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Warehouse documents select request
/// </summary>
public class WarehouseDocumentsSelectRequestModel : DocumentsSelectRequestBaseModel
{
    /// <summary>
    /// DisabledOnly
    /// </summary>
    public bool? DisabledOnly { get; set; }

    /// <summary>
    /// Дата доставки должна быть не меньше указанной
    /// </summary>
    public DateTime? AfterDeliveryDate { get; set; }
}