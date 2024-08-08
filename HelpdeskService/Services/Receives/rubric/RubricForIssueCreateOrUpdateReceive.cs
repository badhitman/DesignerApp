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
        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        if (await context.RubricsForIssues.AnyAsync(x => x.ParentRubricId == rubric.ParentRubricId && x.Name == rubric.Name))
        {
            res.AddError("Рубрика с таким именем уже существует");
            return res;
        }
        await context.AddAsync(rubric);
        await context.SaveChangesAsync();

        res.Response = rubric.Id;
        res.AddSuccess("Рубрика успешна создана");
        return res;
    }
}