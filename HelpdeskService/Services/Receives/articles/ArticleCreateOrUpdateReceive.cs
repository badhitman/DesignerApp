////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// ArticleCreateOrUpdateReceive
/// </summary>
public class ArticleCreateOrUpdateReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory, ILogger<ArticleCreateOrUpdateReceive> loggerRepo)
    : IResponseReceive<ArticleModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ArticleUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(ArticleModelDB? article)
    {
        ArgumentNullException.ThrowIfNull(article);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(article)}");
        TResponseModel<int?> res = new();
        article.Name = article.Name.Trim();
        if (string.IsNullOrWhiteSpace(article.Name))
        {
            res.AddError("Статья должна иметь название");
            return res;
        }
        article.NormalizedNameUpper = article.Name.ToUpper();
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        if (article.Id < 1)
        {
            article.Id = 0;
            article.CreatedAtUTC = DateTime.UtcNow;

            await context.AddAsync(article);
            await context.SaveChangesAsync();
            res.Response = article.Id;
            return res;
        }
        DateTime dtu = DateTime.UtcNow;
        res.Response = await context.Articles
            .Where(a => a.Id == article.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.UpdatedAtUTC, dtu)
            .SetProperty(p => p.Name, article.Name)
            .SetProperty(p => p.Description, article.Description)
            .SetProperty(p => p.NormalizedNameUpper, article.NormalizedNameUpper));

        return res;
    }
}