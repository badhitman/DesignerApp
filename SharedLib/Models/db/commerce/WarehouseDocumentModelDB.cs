////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// WarehouseDocumentModelDB
/// </summary>
[Index(nameof(DeliveryData))]
public class WarehouseDocumentModelDB : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// Дата доставки
    /// </summary>
    public required DateTime DeliveryData { get; set; }

    /// <summary>
    /// Идентификатор документа из внешней системы (например 1С)
    /// </summary>
    public string? ExternalDocumentId { get; set; }

    /// <summary>
    /// Rows
    /// </summary>
    List<RowOfWarehouseDocumentModelDB>? Rows { get; set; }
}