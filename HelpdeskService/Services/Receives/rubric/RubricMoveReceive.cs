////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Сдвинуть рубрику
/// </summary>
public class RubricMoveReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<RowMoveModel?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricForIssuesMoveHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(RowMoveModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool?> res = new();

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        var data = await context
        .RubricsForIssues
        .Where(x => x.Id == req.ObjectId)
        .Select(x => new { x.Id, x.ParentRubricId, x.Name })
        .FirstAsync(x => x.Id == req.ObjectId);

        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        LockUniqueTokenModelDB locker = new() { Token = $"rubric-sort-upd-{data.ParentRubricId}" };
        try
        {
            await context.AddAsync(locker);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            res.AddError($"Не удалось выполнить команду: {ex.Message}");
            return res;
        }

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
                RubricIssueHelpdeskModelDB r1 = all[i - 1], r2 = all[i];
                uint val1 = r1.SortIndex, val2 = r2.SortIndex;
                r1.SortIndex = uint.MaxValue;
                context.Update(r1);
                await context.SaveChangesAsync();
                r2.SortIndex = val1;
                context.Update(r2);
                await context.SaveChangesAsync();
                r1.SortIndex = val2;
                context.Update(r1);
                await context.SaveChangesAsync();
                //r1.SortIndex++;
                //r2.SortIndex--;
                //context.UpdateRange(all);
                //await context.SaveChangesAsync();

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
                RubricIssueHelpdeskModelDB r1 = all[i + 1], r2 = all[i];
                uint val1 = r1.SortIndex, val2 = r2.SortIndex;
                r1.SortIndex = uint.MaxValue;
                context.Update(r1);
                await context.SaveChangesAsync();
                r2.SortIndex = val1;
                context.Update(r2);
                await context.SaveChangesAsync();
                r1.SortIndex = val2;
                context.Update(r1);
                await context.SaveChangesAsync();
                //r1.SortIndex--;
                //r2.SortIndex++;
                //context.UpdateRange(all);
                //await context.SaveChangesAsync();

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
        }
        context.Remove(locker);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return res;
    }
}