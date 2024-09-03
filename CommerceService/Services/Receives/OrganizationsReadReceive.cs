////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrganizationsReadReceive
/// </summary>
public class OrganizationsReadReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
: IResponseReceive<int[]?, OrganizationModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationsReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<OrganizationModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<OrganizationModelDB[]?> res = new();
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        res.Response = await context
            .Organizations
            .Where(x => req.Any(y => y == x.Id))
            .Include(x => x.Addresses)
            .Include(x => x.Users)
            .ToArrayAsync();

        return res;
    }
}
