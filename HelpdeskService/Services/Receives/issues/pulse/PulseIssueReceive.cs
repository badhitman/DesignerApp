////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Регистрация события из обращения (логи).
/// </summary>
/// <remarks>
/// Плюс рассылка уведомлений участникам события.
/// </remarks>
public class PulseIssueReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<PulseIssueBaseModel>?, bool>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PulseIssuePushHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ResponseHandleAction(TAuthRequestModel<PulseIssueBaseModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool> res = new()
        {
            Response = false,
        };

        TResponseModel<IssueHelpdeskModelDB?> issue_data = await helpdeskTransmissionRepo.IssueRead(new TAuthRequestModel<IssueReadRequestModel>()
        {
            SenderActionUserId = GlobalStaticConstants.Roles.System,
            Payload = new() { IssueId = req.Payload.IssueId, IncludeSubscribersOnly = true },
        });

        if (!issue_data.Success() || issue_data.Response is null)
            return new() { Messages = issue_data.Messages };

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        await context.AddAsync(new PulseIssueModelDB()
        {
            AuthorUserIdentityId = req.SenderActionUserId,
            Description = req.Payload.Description,
            CreatedAt = DateTime.UtcNow,
            IssueId = req.Payload.IssueId,
            PulseType = req.Payload.PulseType,
            Tag = req.Payload.Tag,
        });
        await context.SaveChangesAsync();
        res.Response = true;

        if (req.Payload.PulseType != PulseIssuesTypesEnum.Messages && req.Payload.PulseType != PulseIssuesTypesEnum.Status && req.Payload.PulseType != PulseIssuesTypesEnum.Subscribes)
            return res;
        else if ((req.Payload.PulseType == PulseIssuesTypesEnum.Messages || req.Payload.PulseType == PulseIssuesTypesEnum.Subscribes) && req.Payload.Tag != GlobalStaticConstants.Routes.ADD_ACTION_NAME)
            return res;

        List<string> users_ids = [issue_data.Response.AuthorIdentityUserId];
        if (!string.IsNullOrWhiteSpace(issue_data.Response.ExecutorIdentityUserId))
            users_ids.Add(issue_data.Response.ExecutorIdentityUserId);
        if (issue_data.Response.Subscribers is not null && issue_data.Response.Subscribers.Count != 0)
            users_ids.AddRange(issue_data.Response.Subscribers.Where(x => !x.IsSilent).Select(x => x.UserId));

        users_ids = [.. users_ids.Distinct()];
        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.GetUsersIdentity([.. users_ids]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != users_ids.Count)
            return new() { Messages = rest.Messages };

        foreach (UserInfoModel user in rest.Response)
            await webTransmissionRepo.SendEmail(new() { Email = user.Email!, Subject = $"Уведомление: {req.Payload.PulseType.DescriptionInfo()}", TextMessage = req.Payload.Description });

        return res;
    }
}