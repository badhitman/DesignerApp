////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Commerce
/// </summary>
public partial interface ICommerceService
{
    /// <summary>
    /// Get order report file
    /// </summary>
    public Task<TResponseModel<FileAttachModel>> GetOrderReportFile(TAuthRequestModel<int> req);

    /// <summary>
    /// Смена статуса заказу
    /// </summary>
    public Task<TResponseModel<bool>> StatusOrderChange(StatusOrderChangeRequestModel req);

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