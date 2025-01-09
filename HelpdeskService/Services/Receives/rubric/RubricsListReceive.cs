////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Получить рубрики, вложенные в рубрику (если не указано, то root перечень)
/// </summary>
public class RubricsListReceive(IHelpdeskService hdRepo)
    : IResponseReceive<RubricsListRequestModel?, List<UniversalBaseModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricsForIssuesListHelpdeskReceive;

    /// <summary>
    /// Получить рубрики, вложенные в рубрику <paramref name="req"/>.OwnerId (если не указано, то root перечень)
    /// </summary>
    /// <param name="req">OwnerId: вышестоящая рубрика.</param>
    /// <returns>Рубрики, подчинённые <c>OwnerId</c></returns>
    public async Task<List<UniversalBaseModel>?> ResponseHandleAction(RubricsListRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await hdRepo.RubricsList(req);
    }
}