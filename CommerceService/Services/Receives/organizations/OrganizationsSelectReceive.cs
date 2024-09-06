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

        TResponseModel<TPaginationResponseModel<OrganizationModelDB>?> res = new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
            }
        };

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OrganizationModelDB> q = context
            .Organizations
            .AsQueryable();

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate);

        if (!string.IsNullOrWhiteSpace(req.Payload.ForUserIdentityId))
            q = q.Where(x => context.OrganizationsUsers.Any(y => y.OrganizationId == x.Id && y.UserPersonIdentityId == req.Payload.ForUserIdentityId));

        q = q.OrderBy(x => x.LastAtUpdatedUTC)
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        res.Response.Response = req.Payload.IncludeExternalData
            ? [.. await q.Include(x => x.Addresses).ToArrayAsync()]
            : [.. await q.ToArrayAsync()];

        return res;
    }
}