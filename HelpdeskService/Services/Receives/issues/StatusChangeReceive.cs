////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// StatusChangeReceive
/// </summary>
public class StatusChangeReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<StatusChangeRequestModel>?, bool>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.StatusChangeIssueHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ResponseHandleAction(TAuthRequestModel<StatusChangeRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool> res = new()
        {
            Response = false,
        };

        TResponseModel<UserInfoModel[]?> rest = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        UserInfoModel actor = rest.Response[0];

        TResponseModel<IssueHelpdeskModelDB?> issue_data = await helpdeskTransmissionRepo.IssueRead(new TAuthRequestModel<IssueReadRequestModel>()
        {
            SenderActionUserId = actor.UserId,
            Payload = new() { IssueId = req.Payload.IssueId, IncludeSubscribersOnly = true },
        });

        if (!issue_data.Success() || issue_data.Response is null)
            return new() { Messages = issue_data.Messages };

        if (!actor.IsAdmin &&
            issue_data.Response.AuthorIdentityUserId != actor.UserId &&
            issue_data.Response.ExecutorIdentityUserId != actor.UserId &&
            actor.UserId != GlobalStaticConstants.Roles.System &&
            actor.UserId != GlobalStaticConstants.Roles.HelpDeskTelegramBotManager)
        {
            res.AddError("Не достаточно прав для смены статуса");
            return res;
        }

        if (req.SenderActionUserId != GlobalStaticConstants.Roles.System && issue_data.Response.Subscribers?.Any(x => x.UserId == req.SenderActionUserId) != true)
        {
            await helpdeskTransmissionRepo.SubscribeUpdate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    IssueId = issue_data.Response.Id,
                    SetValue = true,
                    UserId = actor.UserId,
                    IsSilent = false,
                }
            });
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();

        if (issue_data.Response.StepIssue == req.Payload.Step)
            res.AddInfo("Статус уже установлен");
        else
        {
            if (string.IsNullOrWhiteSpace(
                issue_data.Response.ExecutorIdentityUserId) && 
                req.Payload.Step >= HelpdeskIssueStepsEnum.Progress && 
                req.Payload.Step != HelpdeskIssueStepsEnum.Canceled)
            {
                res.AddError("Для перевода обращения в работу нужно сначала указать исполнителя");
                return res;
            }

            string msg = $"Статус успешно изменён с `{issue_data.Response.StepIssue}` на `{req.Payload.Step}`";

            await context
                .Issues
                .Where(x => x.Id == issue_data.Response.Id)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.StepIssue, req.Payload.Step));

            res.AddSuccess(msg);
            res.Response = true;
            await helpdeskTransmissionRepo.PulsePush(new()
            {
                SenderActionUserId = req.SenderActionUserId,
                Payload = new()
                {
                    IssueId = issue_data.Response.Id,
                    PulseType = PulseIssuesTypesEnum.Status,
                    Tag = req.Payload.Step.DescriptionInfo(),
                    Description = msg,
                }
            });
        }

        return res;
    }
}