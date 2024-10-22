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
/// Subscribe update - of context user
/// </summary>
public class ExecuterUpdateReceive(
    IDbContextFactory<HelpdeskContext> helpdeskDbFactory,
    ILogger<ExecuterUpdateReceive> loggerRepo,
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
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<bool> res = new();

        TResponseModel<IssueHelpdeskModelDB[]> issues_data = await helpdeskTransmissionRepo.IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
        {
            SenderActionUserId = req.SenderActionUserId,
            Payload = new IssuesReadRequestModel()
            {
                IssuesIds = [req.Payload.IssueId],
                IncludeSubscribersOnly = true,
            },
        });

        if (!issues_data.Success() || issues_data.Response is null || issues_data.Response.Length == 0)
            return new() { Messages = issues_data.Messages };

        IssueHelpdeskModelDB issue_data = issues_data.Response.Single();

        string[] users_ids = [req.SenderActionUserId, req.Payload.UserId, issue_data.ExecutorIdentityUserId ?? ""];
        users_ids = [.. users_ids.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()];

        TResponseModel<UserInfoModel[]?> users_rest = await webTransmissionRepo.GetUsersIdentity(users_ids);
        if (!users_rest.Success() || users_rest.Response is null || users_rest.Response.Length != users_ids.Length)
            return new() { Messages = users_rest.Messages };

        UserInfoModel actor = users_rest.Response.First(x => x.UserId == req.SenderActionUserId);
        UserInfoModel? requested_user = users_rest.Response.FirstOrDefault(x => x.UserId == req.Payload.UserId);

        if (req.SenderActionUserId != GlobalStaticConstants.Roles.System && issue_data.Subscribers?.Any(x => x.UserId == req.SenderActionUserId) != true)
        {
            await helpdeskTransmissionRepo.SubscribeUpdate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    IssueId = issue_data.Id,
                    SetValue = true,
                    UserId = actor.UserId,
                    IsSilent = false,
                }
            });
        }
        if (issue_data.Subscribers?.Any(x => x.UserId == req.Payload.UserId) != true)
        {
            await helpdeskTransmissionRepo.SubscribeUpdate(new()
            {
                SenderActionUserId = GlobalStaticConstants.Roles.System,
                Payload = new()
                {
                    IssueId = issue_data.Id,
                    SetValue = true,
                    UserId = req.Payload.UserId,
                    IsSilent = false,
                }
            });
        }

        using HelpdeskContext context = await helpdeskDbFactory.CreateDbContextAsync();
        string msg;
        if (string.IsNullOrWhiteSpace(req.Payload.UserId))
        {
            if (string.IsNullOrWhiteSpace(issue_data.ExecutorIdentityUserId))
                res.AddInfo("Исполнитель уже отсутствует");
            else
            {
                if (issue_data.StepIssue >= StatusesDocumentsEnum.Progress)
                {
                    res.AddError($"Обращение в статусе [{issue_data.StepIssue.DescriptionInfo()}]. После того как обращение переходит в работу (и далее) удалить исполнителя нельзя. Для открепления исполнителя поставьте обращение на паузу");
                    return res;
                }

                await context
                    .Issues
                    .Where(x => x.Id == req.Payload.IssueId)
                    .ExecuteUpdateAsync(set => set.SetProperty(b => b.ExecutorIdentityUserId, req.Payload.UserId));

                msg = $"Исполнитель `{users_rest.Response.First(x => x.UserId == issue_data.ExecutorIdentityUserId).UserName}` успешно откреплён от обращения";
                res.AddSuccess(msg);

                PulseRequestModel p_req = new()
                {
                    Payload = new()
                    {
                        Payload = new()
                        {
                            IssueId = issue_data.Id,
                            PulseType = PulseIssuesTypesEnum.Executor,
                            Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                            Description = msg,
                        },
                        SenderActionUserId = req.SenderActionUserId
                    }
                };

                await helpdeskTransmissionRepo.PulsePush(p_req);
            }
        }
        else
        {
            if (issue_data.ExecutorIdentityUserId == req.Payload.UserId)
                res.AddInfo($"Исполнитель `{requested_user!.UserName}` уже установлен");
            else
            {
                // msg = $"Исполнитель обращения успешно установлен: {requested_user!.UserName}";
                msg = "Исполнитель обращения успешно";
                PulseRequestModel p_req;
                if (string.IsNullOrWhiteSpace(issue_data.ExecutorIdentityUserId))
                {
                    msg += $": установлен `{requested_user?.UserName}`";
                    p_req = new()
                    {
                        Payload = new()
                        {
                            Payload = new()
                            {
                                IssueId = issue_data.Id,
                                PulseType = PulseIssuesTypesEnum.Executor,
                                Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                                Description = msg,
                            },
                            SenderActionUserId = req.SenderActionUserId
                        }
                    };

                    await helpdeskTransmissionRepo.PulsePush(p_req);
                }
                else
                {
                    msg += $": изменён `{users_rest.Response.FirstOrDefault(x => x.UserId == issue_data.ExecutorIdentityUserId)}` в `{requested_user?.UserName}`";

                    p_req = new()
                    {
                        Payload = new()
                        {
                            Payload = new()
                            {
                                IssueId = issue_data.Id,
                                PulseType = PulseIssuesTypesEnum.Executor,
                                Tag = GlobalStaticConstants.Routes.DELETE_ACTION_NAME,
                                Description = msg,
                            },
                            SenderActionUserId = req.SenderActionUserId
                        }
                    };

                    await helpdeskTransmissionRepo.PulsePush(p_req);
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