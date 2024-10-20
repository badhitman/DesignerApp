////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// TagsOfArticlesSelectReceive
/// </summary>
public class TagsOfArticlesSelectReceive(IArticlesService artRepo, ILogger<ArticlesSelectReceive> loggerRepo)
    : IResponseReceive<string?, string[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.TagsOfArticlesSelectReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<string[]?>> ResponseHandleAction(string? req)
    {        
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");

        return new()
        {
            Response = await artRepo.TagsOfArticlesSelect(req)
        };
    }
}