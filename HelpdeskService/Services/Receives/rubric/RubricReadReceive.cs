////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Прочитать рубрику (со всеми вышестоящими владельцами)
/// </summary>
public class RubricReadReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory, IMemoryCache cache)
    : IResponseReceive<int?, List<RubricIssueHelpdeskModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricForIssuesReadHelpdeskReceive;

    static readonly TimeSpan _ts = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Прочитать рубрику (со всеми вышестоящими владельцами)
    /// </summary>
    public async Task<TResponseModel<List<RubricIssueHelpdeskModelDB>?>> ResponseHandleAction(int? rubricId)
    {
        ArgumentNullException.ThrowIfNull(rubricId);
        TResponseModel<List<RubricIssueHelpdeskModelDB>?> res = new();
        string mem_key = $"{QueueName}-{rubricId}";
        if (cache.TryGetValue(mem_key, out List<RubricIssueHelpdeskModelDB>? rubric))
        {
            res.Response = rubric;
            return res;
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        List<RubricIssueHelpdeskModelDB> ctrl = [await context
            .RubricsForIssues
            .Include(x => x.ParentRubric)
            .FirstAsync(x => x.Id == rubricId)];

        RubricIssueHelpdeskModelDB? lpi = ctrl.Last();
        while (lpi.ParentRubric is not null)
        {
            ctrl.Add(await context
            .RubricsForIssues
            .Include(x => x.ParentRubric)
            .ThenInclude(x => x!.NestedRubrics)
            .FirstAsync(x => x.Id == lpi.ParentRubric.Id));
            lpi = ctrl.Last();
        }

        res.Response = ctrl;
        cache.Set(mem_key, res.Response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));
        return res;
    }
}