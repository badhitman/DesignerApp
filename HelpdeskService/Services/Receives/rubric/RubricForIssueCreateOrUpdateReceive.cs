////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// CreateIssueTheme
/// </summary>
public class RubricForIssueCreateOrUpdateReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<RubricIssueHelpdeskModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricForIssuesUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(RubricIssueHelpdeskModelDB? rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);
        TResponseModel<int?> res = new();
        rubric.Name = rubric.Name.Trim();
        if (string.IsNullOrEmpty(rubric.Name))
        {
            res.AddError("Рубрика должна иметь имя");
            return res;
        }
        rubric.NormalizedNameToUpper = rubric.Name.ToUpper();
        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        if (await context.RubricsForIssues.AnyAsync(x => x.Id != rubric.Id && x.ParentRubricId == rubric.ParentRubricId && x.Name == rubric.Name))
        {
            res.AddError("Рубрика с таким именем уже существует");
            return res;
        }

        if (rubric.Id < 1)
        {
            uint[] six = await context
                            .RubricsForIssues
                            .Where(x => x.ParentRubricId == rubric.ParentRubricId)
                            .Select(x => x.SortIndex)
                            .ToArrayAsync();

            rubric.SortIndex = six.Length == 0 ? 1 : six.Max() + 1;

            await context.AddAsync(rubric);
            res.AddSuccess("Рубрика успешна создана");
        }
        else
        {
            RubricIssueHelpdeskModelDB rubric_db = await context
                            .RubricsForIssues
                            .FirstAsync(x => x.Id == rubric.Id);

            rubric_db.ParentRubricId = rubric.ParentRubricId;
            rubric_db.ProjectId = rubric.ProjectId;
            rubric_db.Description = rubric.Description;
            rubric_db.Name = rubric.Name;
            rubric_db.IsDisabled = rubric.IsDisabled;

            context.Update(rubric_db);
            res.AddSuccess("Рубрика успешна обновлена");
        }

        await context.SaveChangesAsync();

        res.Response = rubric.Id;

        return res;
    }
}