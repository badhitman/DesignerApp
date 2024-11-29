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
    public Task<FileAttachModel> GetPriceFile();

    /// <summary>
    /// Сохранить заказ в виде файла Excel (*.xlsx)
    /// </summary>
    public byte[] SaveOrderAsExcel(OrderDocumentModelDB orderDb);

    /// <summary>
    /// Get order report file
    /// </summary>
    public Task<TResponseModel<FileAttachModel>> GetOrderReportFile(TAuthRequestModel<int> req);

    /// <summary>
    /// Смена статуса заказу по идентификатору HelpDesk документа
    /// </summary>
    /// <remarks>
    /// В запросе нельзя указывать идентификатор заказа: только идентификатор HelpDesk документа.
    /// Допускается ситуация, когда под одним идентификатором HelpDesk документа могут существовать несколько заказов (объединённые заказы).
    /// </remarks>
    public Task<TResponseModel<bool>> StatusesOrdersChangeByHelpdeskDocumentId(StatusOrderChangeRequestModel req);

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