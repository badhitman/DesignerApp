////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Warehouse
/// </summary>
public partial interface ICommerceService
{
    /// <summary>
    /// Rows for warehouse document delete
    /// </summary>
    public Task<TResponseModel<bool>> RowsForWarehouseDocumentDelete(int[] req);

    /// <summary>
    /// Row for warehouse document update
    /// </summary>
    public Task<TResponseModel<int>> RowForWarehouseDocumentUpdate(RowOfWarehouseDocumentModelDB req);

    /// <summary>
    /// WarehouseDocument update
    /// </summary>
    public Task<TResponseModel<int>> WarehouseDocumentUpdate(WarehouseDocumentModelDB req);

    /// <summary>
    /// WarehouseDocuments select
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<WarehouseDocumentModelDB>>> WarehouseDocumentsSelect(TPaginationRequestModel<WarehouseDocumentsSelectRequestModel> req);

    /// <summary>
    /// WarehouseDocuments read
    /// </summary>
    public Task<TResponseModel<WarehouseDocumentModelDB[]>> WarehouseDocumentsRead(int[] req);

    /// <summary>
    /// Registers select
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OfferAvailabilityModelDB>>> RegistersSelect(TPaginationRequestModel<RegistersSelectRequestBaseModel> req);
}