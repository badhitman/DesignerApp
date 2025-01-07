////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Получить рубрики, вложенные в рубрику (если не указано, то root перечень)
/// </summary>
public class RubricsListReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory) : IResponseReceive<RubricsListRequestModel?, List<UniversalBaseModel>?>
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
        TResponseModel<UniversalBaseModel[]?> res = new();

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IQueryable<UniversalBaseModel> q = context
            .Rubrics
            .Where(x => x.ProjectId == req.ProjectId && x.ContextName == req.ContextName)
            .Select(x => new UniversalBaseModel()
            {
                Name = x.Name,
                Description = x.Description,
                Id = x.Id,
                IsDisabled = x.IsDisabled,
                ParentId = x.ParentId,
                ProjectId = x.ProjectId,
                SortIndex = x.SortIndex,
            })
            .AsQueryable();

        if (req.Request < 1)
            q = q.Where(x => x.ParentId == null || x.ParentId < 1);
        else
            q = q.Where(x => x.ParentId == req.Request);

        return await q.ToListAsync();
    }
}