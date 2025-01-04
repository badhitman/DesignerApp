////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// NomenclaturesReadReceive
/// </summary>
public class NomenclaturesReadReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<int[]?, List<NomenclatureModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.NomenclaturesReadCommerceReceive;

    /// <inheritdoc/>
    public async Task<List<NomenclatureModelDB>?> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        return await context
            .Nomenclatures
            .Where(x => req.Any(y => x.Id == y))
            .Include(x => x.Offers)
            .ToListAsync();
    }
}