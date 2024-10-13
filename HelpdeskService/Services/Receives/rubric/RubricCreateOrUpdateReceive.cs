////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using DbcLib;

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
        rubric.Name = rubric.Name.Trim();
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
            res.AddSuccess("Объект успешно создан");
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
            rubric_db.ContextName = rubric.ContextName;

            context.Update(rubric_db);
            res.AddSuccess("Объект успешно обновлён");
        }

        await context.SaveChangesAsync();

        res.Response = rubric.Id;

        return res;
    }
}