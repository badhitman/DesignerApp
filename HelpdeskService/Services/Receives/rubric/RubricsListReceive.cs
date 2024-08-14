////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Получить рубрики, вложенные в рубрику (если не указано, то root перечень)
/// </summary>
public class RubricsListReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<ProjectOwnedRequestModel?, RubricIssueHelpdeskLowModel[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricsForIssuesListHelpdeskReceive;

    /// <summary>
    /// Получить рубрики, вложенные в рубрику <paramref name="req"/>.OwnerId (если не указано, то root перечень)
    /// </summary>
    /// <param name="req">OwnerId: вышестоящая рубрика.</param>
    /// <returns>Рубрики, подчинённые <c>OwnerId</c></returns>
    public async Task<TResponseModel<RubricIssueHelpdeskLowModel[]?>> ResponseHandleAction(ProjectOwnedRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<RubricIssueHelpdeskLowModel[]?> res = new();

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IQueryable<RubricIssueHelpdeskLowModel> q = context
            .RubricsForIssues
            .Where(x => x.ProjectId == req.ProjectId)
            .Select(x => new RubricIssueHelpdeskLowModel() { Name = x.Name, Description = x.Description, Id = x.Id, IsDisabled = x.IsDisabled, ParentRubricId = x.ParentRubricId, ProjectId = x.ProjectId, SortIndex = x.SortIndex })
            .AsQueryable();

        if (req.OwnerId is null || req.OwnerId < 1)
            q = q.Where(x => x.ParentRubricId == null || x.ParentRubricId < 1);
        else
            q = q.Where(x => x.ParentRubricId == req.OwnerId);

        res.Response = await q.ToArrayAsync();
        return res;
    }
}