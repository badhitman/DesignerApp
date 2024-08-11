////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// CreateIssue
/// </summary>
public class IssueCreateOrUpdateReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory)
    : IResponseReceive<IssueHelpdeskModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssueUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(IssueHelpdeskModelDB? issue)
    {
        ArgumentNullException.ThrowIfNull(issue);
        TResponseModel<int?> res = new();
        issue.Description = issue.Description?.Trim();
        issue.Name = issue.Name.Trim();
        issue.NormalizeNameUpper = issue.Name.ToUpper();
        issue.LastUpdateAt = DateTime.UtcNow;

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        if (issue.Id < 1)
        {
            issue.CreatedAt = DateTime.UtcNow;
            await context.AddAsync(issue);
            res.AddSuccess("Обращение успешно создано");
        }
        else
        {
            context.Update(issue);
            res.AddSuccess("Обращение успешно обновлено");
        }
        res.Response = issue.Id;
        await context.SaveChangesAsync();
        return res;
    }
}