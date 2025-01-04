////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// ArticlesSelectReceive
/// </summary>
public class ArticlesSelectReceive(IArticlesService artRepo, ILogger<ArticlesSelectReceive> loggerRepo)
    : IResponseReceive<TPaginationRequestModel<SelectArticlesRequestModel>?, TPaginationResponseModel<ArticleModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ArticlesSelectHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<ArticleModelDB>?>> ResponseHandleAction(TPaginationRequestModel<SelectArticlesRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");

        return new()
        {
            Response = await artRepo.ArticlesSelect(req)
        };
    }
}