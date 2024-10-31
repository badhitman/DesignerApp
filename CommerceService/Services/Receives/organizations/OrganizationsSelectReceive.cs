////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrganizationsSelectReceive
/// </summary>
public class OrganizationsSelectReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
: IResponseReceive<TPaginationRequestModel<OrganizationsSelectRequestModel>?, TPaginationResponseModel<OrganizationModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationsSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>?>> ResponseHandleAction(TPaginationRequestModel<OrganizationsSelectRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OrganizationModelDB> q = context
            .Organizations
            .AsQueryable();

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate);

        if (!string.IsNullOrWhiteSpace(req.Payload.ForUserIdentityId))
            q = q.Where(x => context.OrganizationsUsers.Any(y => y.OrganizationId == x.Id && y.UserPersonIdentityId == req.Payload.ForUserIdentityId));

        q = req.SortingDirection == VerticalDirectionsEnum.Up
            ? q.OrderBy(x => x.LastAtUpdatedUTC)
            : q.OrderByDescending(x => x.LastAtUpdatedUTC);

        IQueryable<OrganizationModelDB> pq = q.OrderBy(x => x.LastAtUpdatedUTC)
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        var extQ = pq.Include(x => x.Addresses).Include(x => x.Users);

        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = req.Payload.IncludeExternalData ? [.. await extQ.ToArrayAsync()] : [.. await pq.ToArrayAsync()]
            }
        }; ;
    }
}