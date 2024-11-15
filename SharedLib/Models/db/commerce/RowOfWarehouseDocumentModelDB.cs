////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

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
}