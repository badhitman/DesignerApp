////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// RowOfWarehouseDocumentModelDB
/// </summary>
public class RowOfWarehouseDocumentModelDB : RowOfBaseDocumentModelDB
{
    /// <summary>
    /// WarehouseDocument
    /// </summary>
    public WarehouseDocumentModelDB? WarehouseDocument { get; set; }
    /// <summary>
    /// WarehouseDocument
    /// </summary>
    public int WarehouseDocumentId { get; set; }

    /// <summary>
    /// Version
    /// </summary>
    [ConcurrencyCheck]
    public Guid Version { get; set; }
}