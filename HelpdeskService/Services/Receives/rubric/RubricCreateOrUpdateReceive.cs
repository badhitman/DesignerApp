////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using DbcLib;
using System.Text.RegularExpressions;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// CreateIssueTheme
/// </summary>
public class RubricCreateOrUpdateReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory, ILogger<RubricCreateOrUpdateReceive> loggerRepo)
    : IResponseReceive<RubricIssueHelpdeskModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.RubricForIssuesUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(RubricIssueHelpdeskModelDB? rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(rubric)}");
        TResponseModel<int?> res = new();
        Regex rx = new(@"\s+", RegexOptions.Compiled);
        rubric.Name = rx.Replace(rubric.Name.Trim(), " ");
        if (string.IsNullOrWhiteSpace(rubric.Name))
        {
            res.AddError("Объект должен иметь имя");
            return res;
        }
        rubric.NormalizedNameUpper = rubric.Name.ToUpper();
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        if (await context.RubricsForIssues.AnyAsync(x => x.Id != rubric.Id && x.ParentRubricId == rubric.ParentRubricId && x.Name == rubric.Name))
        {
            res.AddError("Объект с таким именем уже существует в данном узле");
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
            await context.SaveChangesAsync();
            res.AddSuccess("Объект успешно создан");
            res.Response = rubric.Id;
        }
        else
        {
            res.Response = await context
                .RubricsForIssues
                .Where(x => x.Id == rubric.Id)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.IsDisabled, rubric.IsDisabled)
                .SetProperty(p => p.Name, rubric.Name)
                .SetProperty(p => p.Description, rubric.Description));

            res.AddSuccess("Объект успешно обновлён");
        }

        return res;
    }
}