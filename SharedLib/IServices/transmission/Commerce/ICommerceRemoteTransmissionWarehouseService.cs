﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

public partial interface ICommerceRemoteTransmissionService
{
    /// <summary>
    /// Получить остатки
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OfferAvailabilityModelDB>>> OffersRegistersSelect(TPaginationRequestModel<RegistersSelectRequestBaseModel> req);

    /// <summary>
    /// Удалить строку складского документа
    /// </summary>
    public Task<TResponseModel<bool>> RowsForWarehouseDelete(int[] req);

    /// <summary>
    /// Обновить строку складского документа
    /// </summary>
    public Task<TResponseModel<int>> RowForWarehouseUpdate(RowOfWarehouseDocumentModelDB row);

    /// <summary>
    /// WarehousesRead
    /// </summary>
    public Task<TResponseModel<WarehouseDocumentModelDB[]>> WarehousesRead(int[] ids);

    /// <summary>
    /// WarehouseUpdate
    /// </summary>
    public Task<TResponseModel<int>> WarehouseUpdate(WarehouseDocumentModelDB document);

    /// <summary>
    /// Подбор складских документов (поиск по параметрам)
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<WarehouseDocumentModelDB>>> WarehousesSelect(TPaginationRequestModel<WarehouseDocumentsSelectRequestModel> req);
}