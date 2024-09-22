////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Read issue - of context user
/// </summary>
public class IssuesReadReceive(
    ILogger<IssuesReadReceive> LoggerRepo,
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    IMemoryCache cache)
    : IResponseReceive<TAuthRequestModel<IssuesReadRequestModel>?, IssueHelpdeskModelDB[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.IssuesGetHelpdeskReceive;

    static readonly TimeSpan _ts = TimeSpan.FromSeconds(5);

    /// <inheritdoc/>
    public async Task<TResponseModel<IssueHelpdeskModelDB[]?>> ResponseHandleAction(TAuthRequestModel<IssuesReadRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<IssueHelpdeskModelDB[]?> res = new();
        string mem_key = $"{QueueName}-{string.Join(";", req.Payload.IssuesIds)}/{req.Payload.IncludeSubscribersOnly}({req.SenderActionUserId})";
        if (cache.TryGetValue(mem_key, out IssueHelpdeskModelDB[]? hd))
        {
            res.Response = hd;
            return res;
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        IIncludableQueryable<IssueHelpdeskModelDB, List<SubscriberIssueHelpdeskModelDB>?> q = context.Issues.Include(x => x.Subscribers);
        IssueHelpdeskModelDB[]? issues_db = req.Payload.IncludeSubscribersOnly
            ? await q.Where(x => req.Payload.IssuesIds.Any(y => y == x.Id)).ToArrayAsync()
            : await q.Include(x => x.RubricIssue).Include(x => x.Messages).Where(x => req.Payload.IssuesIds.Any(y => y == x.Id)).ToArrayAsync();

        if (issues_db is null || issues_db.Length == 0)
        {
            LoggerRepo.LogError($"Обращение не найдено: {mem_key}");
            return new()
            {
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Обращение не найдено или у вас нет к нему доступа" }]
            };
        }

        if (req.SenderActionUserId == GlobalStaticConstants.Roles.System || issues_db.All(x => x.ExecutorIdentityUserId == req.SenderActionUserId) || issues_db.All(x => x.AuthorIdentityUserId == req.SenderActionUserId) || issues_db.All(x => x.Subscribers!.Any(x => x.UserId == req.SenderActionUserId)))
            return new() { Response = issues_db };

        TResponseModel<UserInfoModel[]?> rest_user_date = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!rest_user_date.Success() || rest_user_date.Response is null || rest_user_date.Response.Length != 1)
        {
            LoggerRepo.LogError($"Пользователь не найден: {req.SenderActionUserId}");
            return new() { Messages = rest_user_date.Messages };
        }

        if (!rest_user_date.Response[0].IsAdmin && rest_user_date.Response[0].Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Contains(x)) != true)
        {
            LoggerRepo.LogError($"Для получения обращений не достаточно прав: {mem_key}");
            return new()
            {
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Обращение не найдено или у вас нет к нему доступа" }]
            };
        }

        cache.Set(mem_key, issues_db, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));
        return new() { Response = issues_db };
    }
}