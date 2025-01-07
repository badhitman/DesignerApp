////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// NomenclaturesSelectReceive
/// </summary>
public class NomenclaturesSelectReceive(IDbContextFactory<CommerceContext> commerceDbFactory) : IResponseReceive<TPaginationRequestModel<NomenclaturesSelectRequestModel>?, TPaginationResponseModel<NomenclatureModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.NomenclaturesSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NomenclatureModelDB>?> ResponseHandleAction(TPaginationRequestModel<NomenclaturesSelectRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        IQueryable<NomenclatureModelDB> q = string.IsNullOrEmpty(req.Payload.ContextName)
            ? context.Nomenclatures.Where(x => x.ContextName == null || x.ContextName == "").AsQueryable()
            : context.Nomenclatures.Where(x => x.ContextName == req.Payload.ContextName).AsQueryable();

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate);

        IOrderedQueryable<NomenclatureModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
          ? q.OrderBy(x => x.CreatedAtUTC)
          : q.OrderByDescending(x => x.CreatedAtUTC);

        return new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortingDirection = req.SortingDirection,
            SortBy = req.SortBy,
            TotalRowsCount = await q.CountAsync(),
            Response = [.. await oq.Skip(req.PageNum * req.PageSize).Take(req.PageSize).ToArrayAsync()]
        };
    }
}