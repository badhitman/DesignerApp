////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////


namespace SharedLib;

/// <summary>
/// RowOrderDocumentRecord
/// </summary>
public record RowOrderDocumentRecord(int DocumentId, StatusesDocumentsEnum DocumentStatus, int OfferId, int GoodsId, decimal Quantity, int WarehouseId);