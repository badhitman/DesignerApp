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
        if (string.IsNullOrEmpty(article.Name))
        {
            res.AddError("Объект должен иметь имя");
            return res;
        }
        article.NormalizedNameUpper = article.Name.ToUpper();
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        res.Response = article.Id;

        return res;
    }
}