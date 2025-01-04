////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.storage;

/// <summary>
/// TagsSelectReceive
/// </summary>
public class TagsSelectReceive(ILogger<TagsSelectReceive> loggerRepo, IDbContextFactory<StorageContext> cloudParametersDbFactory)
    : IResponseReceive<TPaginationRequestModel<SelectMetadataRequestModel>?, TPaginationResponseModel<TagModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.TagsSelectReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<TagModelDB>?>> ResponseHandleAction(TPaginationRequestModel<SelectMetadataRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");

        if (req.PageSize < 5)
            req.PageSize = 5;
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();

        IQueryable<TagModelDB> q = context
            .CloudTags
            .AsQueryable();

        if (req.Payload.ApplicationsNames is not null && req.Payload.ApplicationsNames.Length != 0)
            q = q.Where(x => req.Payload.ApplicationsNames.Any(y => y == x.ApplicationName));

        if (!string.IsNullOrWhiteSpace(req.Payload.PropertyName))
            q = q.Where(x => x.PropertyName == req.Payload.PropertyName);

        if (!string.IsNullOrWhiteSpace(req.Payload.PrefixPropertyName))
            q = q.Where(x => x.PrefixPropertyName == req.Payload.PrefixPropertyName);

        if (req.Payload.OwnerPrimaryKey.HasValue && req.Payload.OwnerPrimaryKey.Value > 0)
            q = q.Where(x => x.OwnerPrimaryKey == req.Payload.OwnerPrimaryKey.Value);

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
            q = q.Where(x => x.NormalizedTagNameUpper!.Contains(req.Payload.SearchQuery.ToUpper()));

        IQueryable<TagModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
          ? q.OrderBy(x => x.TagName).Skip(req.PageNum * req.PageSize).Take(req.PageSize)
          : q.OrderByDescending(x => x.TagName).Skip(req.PageNum * req.PageSize).Take(req.PageSize);

        int trc = await q.CountAsync();
        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = trc,
                Response = await oq.ToListAsync()
            }
        };
    }
}