////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace CommerceService;

record RowOrderDocumentRecord(int DocumentId, StatusesDocumentsEnum DocumentStatus, int OfferId, int GoodsId, decimal Quantity, int WarehouseId);