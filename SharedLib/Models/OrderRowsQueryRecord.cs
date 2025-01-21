////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// OrderRowsQueryRecord
/// </summary>
public record OrderRowsQueryRecord(OrderDocumentModelDB Document, TabAddressForOrderModelDb TabAddress, RowOfOrderDocumentModelDB Row, OfferModelDB Offer, NomenclatureModelDB Goods);