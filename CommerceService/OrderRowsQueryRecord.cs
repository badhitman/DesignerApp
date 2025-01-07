////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace CommerceService;

record OrderRowsQueryRecord(OrderDocumentModelDB Document, TabAddressForOrderModelDb TabAddress, RowOfOrderDocumentModelDB Row, OfferModelDB Offer, NomenclatureModelDB Goods);