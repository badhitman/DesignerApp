////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AddressesOrganizationsReadReceive
/// </summary>
public class AddressesOrganizationsReadReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
: IResponseReceive<int[]?, AddressOrganizationModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddressesOrganizationsReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<AddressOrganizationModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<AddressOrganizationModelDB[]?> res = new();
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        res.Response = await context
            .AddressesOrganizations
            .Where(x => req.Any(y => y == x.Id))
            .ToArrayAsync();

        return res;
    }
}