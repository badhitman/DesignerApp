////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// ArticleCreateOrUpdateReceive
/// </summary>
public class ArticleCreateOrUpdateReceive(IArticlesService artRepo, ILogger<ArticleCreateOrUpdateReceive> loggerRepo)
    : IResponseReceive<ArticleModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ArticleUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(ArticleModelDB? article)
    {
        ArgumentNullException.ThrowIfNull(article);
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(article)}");
        TResponseModel<int> res = await artRepo.ArticleCreateOrUpdate(article);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages,
        };
    }
}