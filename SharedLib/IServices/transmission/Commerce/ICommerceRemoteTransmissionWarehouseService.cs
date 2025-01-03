////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

public partial interface ICommerceRemoteTransmissionService
{
    /// <summary>
    /// Получить остатки
    /// </summary>
    public Task<TPaginationResponseModel<OfferAvailabilityModelDB>> OffersRegistersSelect(TPaginationRequestModel<RegistersSelectRequestBaseModel> req);

    /// <summary>
    /// Удалить строку складского документа
    /// </summary>
    public Task<bool> RowsForWarehouseDelete(int[] req);

    /// <summary>
    /// Обновить строку складского документа
    /// </summary>
    public Task<int> RowForWarehouseUpdate(RowOfWarehouseDocumentModelDB row);

    /// <summary>
    /// WarehousesRead
    /// </summary>
    public Task<WarehouseDocumentModelDB[]> WarehousesRead(int[] ids);

    /// <summary>
    /// WarehouseUpdate
    /// </summary>
    public Task<int> WarehouseUpdate(WarehouseDocumentModelDB document);

    /// <summary>
    /// Подбор складских документов (поиск по параметрам)
    /// </summary>
    public Task<TPaginationResponseModel<WarehouseDocumentModelDB>> WarehousesSelect(TPaginationRequestModel<WarehouseDocumentsSelectRequestModel> req);
}