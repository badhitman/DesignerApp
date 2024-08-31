////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Read issue - of context user
/// </summary>
public class IssueReadReceive(IDbContextFactory<HelpdeskContext> helpdeskDbFactory, IWebRemoteTransmissionService webTransmissionRepo, IMemoryCache cache)
    : IResponseReceive<TAuthRequestModel<IssueReadRequestModel>?, IssueHelpdeskModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssueGetHelpdeskReceive;

    static readonly TimeSpan _ts = TimeSpan.FromSeconds(5);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB?>> ResponseHandleAction(TAuthRequestModel<IssueReadRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<IssueHelpdeskModelDB?> res = new();
        string mem_key = $"{QueueName}-{req.Payload.IssueId}/{req.Payload.IncludeSubscribersOnly}";
        if (cache.TryGetValue(mem_key, out IssueHelpdeskModelDB? hd))
        {
            res.Response = hd;
            return res;
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IIncludableQueryable<IssueHelpdeskModelDB, List<SubscriberIssueHelpdeskModelDB>?> q = context.Issues.Include(x => x.Subscribers);
        IssueHelpdeskModelDB? issue_db = req.Payload.IncludeSubscribersOnly
            ? await q.FirstOrDefaultAsync(x => x.Id == req.Payload.IssueId)
            : await q.Include(x => x.RubricIssue).Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == req.Payload.IssueId);

        if (issue_db is null)
            return new()
            {
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Обращение не найдено или у вас нет к нему доступа" }]
            };

        if (req.SenderActionUserId == GlobalStaticConstants.Roles.System || issue_db.ExecutorIdentityUserId == req.SenderActionUserId || issue_db.AuthorIdentityUserId == req.SenderActionUserId || issue_db.Subscribers!.Any(x => x.UserId == req.SenderActionUserId))
            return new() { Response = issue_db };

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };


        if (!rest.Response[0].IsAdmin && rest.Response[0].Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Contains(x)) != true)
            return new()
            {
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Обращение не найдено или у вас нет к нему доступа" }]
            };

        cache.Set(mem_key, issue_db, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));

        return new()
        {
            Response = issue_db
        };
    }
}