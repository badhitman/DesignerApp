////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Сдвинуть рубрику
/// </summary>
public class RubricForIssuesMoveReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<RowMoveModel?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricForIssuesMoveHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(RowMoveModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool?> res = new();

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        RubricIssueHelpdeskModelDB data = await context
            .RubricsForIssues
            .FirstAsync(x => x.Id == req.ObjectId);

        List<RubricIssueHelpdeskModelDB> all = await context
            .RubricsForIssues
            .Where(x => x.ParentRubricId == data.ParentRubricId)
            .OrderBy(x => x.SortIndex)
            .ToListAsync();

        int i = all.FindIndex(x => x.Id == data.Id);
        if (req.Direction == VerticalDirectionsEnum.Up)
        {
            if (i == 0)
            {
                res.Response = false;
                res.AddInfo("Элемент уже в крайнем положении.");
            }
            else
            {
                all[i - 1].SortIndex++;
                all[i].SortIndex--;
                context.UpdateRange(all[i - 1], all[i]);
                await context.SaveChangesAsync();
                res.Response = true;
                res.AddSuccess($"Рубрика '{data.Name}' сдвинута выше");
            }
        }
        else
        {
            if (i == all.Count - 1)
            {
                res.Response = false;
                res.AddInfo("Элемент уже в крайнем положении.");
            }
            else
            {
                all[i + 1].SortIndex--;
                all[i].SortIndex++;
                context.UpdateRange(all[i + 1], all[i]);
                await context.SaveChangesAsync();
                res.Response = true;
                res.AddSuccess($"Рубрика '{data.Name}' сдвинута ниже");
            }
        }

        all = [.. all.OrderBy(x => x.SortIndex)];

        bool nu = false;
        uint si = 0;
        all.ForEach(x =>
        {
            si++;
            nu = nu || x.SortIndex != si;
            x.SortIndex = si;
        });

        if (nu)
        {
            context.UpdateRange(all);
            await context.SaveChangesAsync();
        }

        return res;
    }
}