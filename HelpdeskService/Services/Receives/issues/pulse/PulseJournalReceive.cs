////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// PulseJournalReceive - of context user
/// </summary>
public class PulseJournalReceive(
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
    public async Task<TResponseModel<TPaginationResponseModel<PulseViewModel>?>> ResponseHandleAction(TPaginationRequestModel<UserIssueModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<TPaginationResponseModel<PulseViewModel>?> res = new();
        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.GetUsersIdentity([req.Payload.UserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        if (req.PageSize < 5)
            req.PageSize = 5;

        UserInfoModel actor = rest.Response[0];

        TResponseModel<IssueHelpdeskModelDB> issue_data = await helpdeskTransmissionRepo.IssueRead(new TAuthRequestModel<IssueReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssueId = req.Payload.IssueId, IncludeSubscribersOnly = true },
        });

        if (!issue_data.Success() || issue_data.Response is null)
            return new() { Messages = issue_data.Messages };

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        IQueryable<PulseIssueModelDB> q = context
            .PulseEvents
            .Where(x => x.IssueId == req.Payload.IssueId);

        IOrderedQueryable<PulseIssueModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Down
            ? q.OrderByDescending(x => x.CreatedAt)
            : q.OrderBy(x => x.CreatedAt);

        res.Response = new()
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

        return res;
    }
}