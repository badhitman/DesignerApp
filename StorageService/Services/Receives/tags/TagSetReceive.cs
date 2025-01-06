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
/// TagSetReceive
/// </summary>
public class TagSetReceive(ILogger<TagSetReceive> loggerRepo, IDbContextFactory<StorageContext> cloudParametersDbFactory)
    : IResponseReceive<TagSetModel?, TResponseModel<bool>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.TagSetReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>?> ResponseHandleAction(TagSetModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();

        TResponseModel<bool> res = new() { Response = false };

        IQueryable<TagModelDB> q = context
            .CloudTags
            .Where(x =>
            x.OwnerPrimaryKey == req.Id &&
            x.ApplicationName == req.ApplicationName &&
            x.NormalizedTagNameUpper == req.Name.ToUpper() &&
            x.PropertyName == req.PropertyName &&
            x.PrefixPropertyName == req.PrefixPropertyName);

        if (req.Set)
        {
            if (await q.AnyAsync())
                res.AddInfo("Тег уже установлен");
            else
            {
                await context.AddAsync(new TagModelDB()
                {
                    ApplicationName = req.ApplicationName,
                    TagName = req.Name,
                    PropertyName = req.PropertyName,
                    CreatedAt = DateTime.UtcNow,
                    NormalizedTagNameUpper = req.Name.ToUpper(),
                    PrefixPropertyName = req.PrefixPropertyName,
                    OwnerPrimaryKey = req.Id,
                });
                await context.SaveChangesAsync();
            }
        }
        else
        {
            if (q.Any())
                res.Response = await q.ExecuteDeleteAsync() > 0;
            else
                res.AddInfo("Тег отсутствует");
        }

        return res;
    }
}