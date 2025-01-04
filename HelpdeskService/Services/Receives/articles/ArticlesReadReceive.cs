////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// ArticlesReadReceive
/// </summary>
public class ArticlesReadReceive(IArticlesService artRepo, ILogger<ArticlesReadReceive> loggerRepo)
    : IResponseReceive<int[]?, ArticleModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ArticlesReadReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<ArticleModelDB[]?>> ResponseHandleAction(int[]? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");

        return new()
        {
            Response = await artRepo.ArticlesRead(req)
        };
    }
}