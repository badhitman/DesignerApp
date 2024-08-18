////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.helpdesk;

/// <summary>
/// Subscribe update - of context user
/// </summary>
public class ExecuterUpdateReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    IHelpdeskRemoteTransmissionService helpdeskTransmissionRepo)
    : IResponseReceive<TAuthRequestModel<UserIssueModel>?, bool>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ExecuterIssueUpdateHelpdeskReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ResponseHandleAction(TAuthRequestModel<UserIssueModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool> res = new();

        TResponseModel<IssueHelpdeskModelDB?> issue_data = await helpdeskTransmissionRepo.IssueRead(new TAuthRequestModel<IssueReadRequestModel>()
        {
            SenderActionUserId = req.SenderActionUserId,
            Payload = new IssueReadRequestModel()
            {
                IssueId = req.Payload.IssueId,
                IncludeSubscribersOnly = true,
            },
        });

        if (!issue_data.Success() || issue_data.Response is null)
            return new() { Messages = issue_data.Messages };

        string[] users_ids = [req.SenderActionUserId, req.Payload.UserId, issue_data.Response.ExecutorIdentityUserId ?? ""];
        users_ids = [.. users_ids.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()];

        TResponseModel<UserInfoModel[]?> users_rest = await webTransmissionRepo.FindUsersIdentity(users_ids);
        if (!users_rest.Success() || users_rest.Response is null || users_rest.Response.Length != users_ids.Length)
            return new() { Messages = users_rest.Messages };

        UserInfoModel actor = users_rest.Response.First(x => x.UserId == req.SenderActionUserId);
        UserInfoModel? requested_user = users_rest.Response.FirstOrDefault(x => x.UserId == req.Payload.UserId);

        HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        string msg;
        if (string.IsNullOrWhiteSpace(req.Payload.UserId))
        {
            if (string.IsNullOrWhiteSpace(issue_data.Response.ExecutorIdentityUserId))
                res.AddInfo("Исполнитель уже отсутствует");
            else
            {
                msg = $"Исполнитель `{users_rest.Response.First(x => x.UserId == issue_data.Response.ExecutorIdentityUserId)}` успешно откреплён от обращения";
                res.AddSuccess(msg);

                await context
                    .Issues
                    .Where(x => x.Id == req.Payload.IssueId)
                    .ExecuteUpdateAsync(set => set.SetProperty(b => b.ExecutorIdentityUserId, req.Payload.UserId));

                await helpdeskTransmissionRepo.PulsePush(new()
                {
                    SenderActionUserId = req.SenderActionUserId,
                    Payload = new()
                    {
                        IssueId = issue_data.Response.Id,
                        PulseType = PulseIssuesTypesEnum.Executor,
                        Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                        Description = msg,
                    }
                });
            }
        }
        else
        {
            if (issue_data.Response.ExecutorIdentityUserId == req.Payload.UserId)
                res.AddInfo($"Исполнитель `{requested_user!.UserName}` уже установлен");
            else
            {
                // msg = $"Исполнитель обращения успешно установлен: {requested_user!.UserName}";
                msg = "Исполнитель обращения успешно";

                if (string.IsNullOrWhiteSpace(issue_data.Response.ExecutorIdentityUserId))
                {
                    msg += $": установлен `{requested_user?.UserName}`";
                    await helpdeskTransmissionRepo.PulsePush(new()
                    {
                        SenderActionUserId = req.SenderActionUserId,
                        Payload = new()
                        {
                            IssueId = issue_data.Response.Id,
                            PulseType = PulseIssuesTypesEnum.Executor,
                            Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                            Description = msg,
                        }
                    });
                }
                else
                {
                    msg += $": изменён `{users_rest.Response.FirstOrDefault(x => x.UserId == issue_data.Response.ExecutorIdentityUserId)}` в `{requested_user?.UserName}`";
                    await helpdeskTransmissionRepo.PulsePush(new()
                    {
                        SenderActionUserId = req.SenderActionUserId,
                        Payload = new()
                        {
                            IssueId = issue_data.Response.Id,
                            PulseType = PulseIssuesTypesEnum.Executor,
                            Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                            Description = msg,
                        }
                    });
                }

                await context
                    .Issues
                    .Where(x => x.Id == req.Payload.IssueId)
                    .ExecuteUpdateAsync(set => set.SetProperty(b => b.ExecutorIdentityUserId, req.Payload.UserId));

                res.AddSuccess(msg);
            }
        }

        return res;
    }
}