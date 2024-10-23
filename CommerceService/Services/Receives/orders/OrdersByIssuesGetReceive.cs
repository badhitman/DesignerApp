////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrdersByIssuesGetReceive
/// </summary>
public class OrdersByIssuesGetReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
: IResponseReceive<OrdersByIssuesSelectRequestModel?, OrderDocumentModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrdersByIssuesGetReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]?>> ResponseHandleAction(OrdersByIssuesSelectRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OrderDocumentModelDB> q = context
            .OrdersDocuments
            .AsQueryable();

        if (req.IssueIds.Length != 0)
            q = q.Where(x => req.IssueIds.Any(y => y == x.HelpdeskId));
        else
            return new()
            {
                Response = [],
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Запрос не может быть пустым" }]
            };

        return new()
        {
            Response = await q.ToArrayAsync(),
        };
    }
}