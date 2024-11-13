////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// RowOfWarehouseDocumentAbstractModelDB
/// </summary>
public abstract class RowOfWarehouseDocumentAbstractModelDB : RowOfBaseDocumentModelDB
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
