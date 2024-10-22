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
public class RubricsListReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<RubricsListRequestModel?, RubricBaseModel[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricsForIssuesListHelpdeskReceive;

    /// <summary>
    /// Получить рубрики, вложенные в рубрику <paramref name="req"/>.OwnerId (если не указано, то root перечень)
    /// </summary>
    /// <param name="req">OwnerId: вышестоящая рубрика.</param>
    /// <returns>Рубрики, подчинённые <c>OwnerId</c></returns>
    public async Task<TResponseModel<RubricBaseModel[]?>> ResponseHandleAction(RubricsListRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<RubricBaseModel[]?> res = new();

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IQueryable<RubricBaseModel> q = context
            .Rubrics
            .Where(x => x.ProjectId == req.ProjectId && x.ContextName == req.ContextName)
            .Select(x => new RubricBaseModel()
            {
                Name = x.Name,
                Description = x.Description,
                Id = x.Id,
                IsDisabled = x.IsDisabled,
                ParentRubricId = x.ParentRubricId,
                ProjectId = x.ProjectId,
                SortIndex = x.SortIndex,
            })
            .AsQueryable();

        if (req.Request < 1)
            q = q.Where(x => x.ParentRubricId == null || x.ParentRubricId < 1);
        else
            q = q.Where(x => x.ParentRubricId == req.Request);

        res.Response = await q.ToArrayAsync();
        return res;
    }
}