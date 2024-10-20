////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// TagArticleSetReceive
/// </summary>
public class TagArticleSetReceive(IArticlesService artRepo, ILogger<ArticleCreateOrUpdateReceive> loggerRepo)
    : IResponseReceive<TagArticleSetModel?, EntryModel[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.TagArticleSetReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryModel[]?>> ResponseHandleAction(TagArticleSetModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        
        return new()
        {
            Response = await artRepo.TagArticleSet(req),
        };
    }
}