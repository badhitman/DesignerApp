////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// UpdateRubricsForArticleReceive
/// </summary>
public class UpdateRubricsForArticleReceive(IArticlesService artRepo, ILogger<ArticleCreateOrUpdateReceive> loggerRepo)
    : IResponseReceive<ArticleRubricsSetModel, bool>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricsForArticleSetReceive;

    /// <inheritdoc/>
    public async Task<bool> ResponseHandleAction(ArticleRubricsSetModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");

        return await artRepo.UpdateRubricsForArticle(req);
    }
}