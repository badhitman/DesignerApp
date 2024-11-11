////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Commerce
/// </summary>
public interface ICommerceService
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
    /// Status change
    /// </summary>
    public Task<TResponseModel<bool>> StatusChange(StatusChangeRequestModel req);

    /// <summary>
    /// Rows for order delete
    /// </summary>
    public Task<TResponseModel<bool>> RowsForOrderDelete(int[] req);

    /// <summary>
    /// Row for order update
    /// </summary>
    public Task<TResponseModel<int>> RowForOrderUpdate(RowOfOrderDocumentModelDB req);

    /// <summary>
    /// Order update
    /// </summary>
    public Task<TResponseModel<int>> OrderUpdate(OrderDocumentModelDB req);

    /// <summary>
    /// Orders select
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req);

    /// <summary>
    /// Orders read
    /// </summary>
    public Task<TResponseModel<OrderDocumentModelDB[]>> OrdersRead(int[] req);

    /// <summary>
    /// Orders by issues get
    /// </summary>
    public Task<TResponseModel<OrderDocumentModelDB[]>> OrdersByIssuesGet(OrdersByIssuesSelectRequestModel req);
}