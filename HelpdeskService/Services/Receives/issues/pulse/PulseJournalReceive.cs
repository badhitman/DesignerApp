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
/// PulseJournalReceive - of context user
/// </summary>
public class PulseJournalReceive(
    ILogger<PulseJournalReceive> LoggerRepo,
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TPaginationRequestModel<UserIssueModel>?, TPaginationResponseModel<PulseViewModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PulseJournalHelpdeskReceive;

    /// <summary>
    /// Подписчики на события в обращении/инциденте
    /// </summary>
    public async Task<TPaginationResponseModel<PulseViewModel>?> ResponseHandleAction(TPaginationRequestModel<UserIssueModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.GetUsersIdentity([req.Payload.UserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Response = [] };

        if (req.PageSize < 5)
            req.PageSize = 5;

        UserInfoModel actor = rest.Response[0];

        LoggerRepo.LogDebug($"Запрос журнала активности пользователем: {JsonConvert.SerializeObject(actor)}");

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await helpdeskTransmissionRepo.IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssuesIds = [req.Payload.IssueId], IncludeSubscribersOnly = true },
        });

        if (!issues_data.Success() || issues_data.Response is null || issues_data.Response.Length == 0)
        {
            LoggerRepo.LogWarning($"Запрос журнала активности пользователем {actor.UserId} - отклонён");
            return new() { Response = [] };
        }
        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IQueryable<PulseIssueModelDB> q = context
            .PulseEvents
            .Where(x => x.IssueId == req.Payload.IssueId);

        IOrderedQueryable<PulseIssueModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Down
            ? q.OrderByDescending(x => x.CreatedAt)
            : q.OrderBy(x => x.CreatedAt);

        return new()
        {
            TotalRowsCount = q.Count(),
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection,
            Response = await oq
            .Skip(req.PageSize * req.PageNum)
            .Take(req.PageSize)
            .Select(x => new PulseViewModel()
            {
                AuthorUserIdentityId = x.AuthorUserIdentityId,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                PulseType = x.PulseType,
                Tag = x.Tag,
            })
            .ToListAsync()
        };
    }
}