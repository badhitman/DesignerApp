////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;
using System.Net.Mail;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// ExecutorIssueComponent
/// </summary>
public partial class ExecutorIssueComponent : IssueWrapBaseModel
{
    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRemoteTransmissionRepo { get; set; } = default!;

    UserInfoModel? Executor;
    string? editExecutorEmail;
    bool IsEditMode;

    async Task SetMeAsExecutor()
    {
        await SetExecutor(CurrentUser.UserId);
    }

    async Task SetNewExecutor()
    {
        if (!string.IsNullOrWhiteSpace(editExecutorEmail) && !MailAddress.TryCreate(editExecutorEmail, out _))
            throw new Exception();

        UserInfoModel? user_by_email = null;
        if (!string.IsNullOrWhiteSpace(editExecutorEmail))
        {
            SetBusy();
            user_by_email = await UsersProfilesRepo.FindByEmailAsync(editExecutorEmail);
            IsBusyProgress = false;
            if (user_by_email is null)
            {
                SnackbarRepo.Error($"Пользователь с таким email не найден: {editExecutorEmail}");
                return;
            }
            
            if (user_by_email.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Contains(x)) != true && !user_by_email.IsAdmin)
            {
                SnackbarRepo.Error($"Пользователь {editExecutorEmail} не может быть установлен исполнителем: не является сотрудником");
                return;
            }
        }

        await SetExecutor(user_by_email?.UserId ?? "");
    }

    async Task SetExecutor(string user_id)
    {
        SetBusy();
        TResponseModel<bool> rest = await HelpdeskRepo.ExecuterUpdate(new() { SenderActionUserId = CurrentUser.UserId, Payload = new() { IssueId = Issue.Id, UserId = user_id } });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (!rest.Success())
            return;

        IsEditMode = false;

        Issue.ExecutorIdentityUserId = user_id;

        if (string.IsNullOrWhiteSpace(user_id))
            return;

        UsersIdentityDump ??= [];
        if (UsersIdentityDump.Any(x => x.UserId == user_id) != true)
        {
            SetBusy();
            TResponseModel<UserInfoModel[]?> res_user = await WebRemoteTransmissionRepo.GetUsersIdentity([user_id]);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(res_user.Messages);
            if(!res_user.Success() || res_user.Response is null || res_user.Response.Length != 1)
                return;

            UsersIdentityDump = [.. UsersIdentityDump.Union(res_user.Response)];
        }

        Executor = UsersIdentityDump?.FirstOrDefault(x => x.UserId == Issue.ExecutorIdentityUserId);
        editExecutorEmail = Executor?.Email ?? Issue.ExecutorIdentityUserId;
    }

    void EditModeToggle()
    {
        IsEditMode = !IsEditMode;
        editExecutorEmail = IsEditMode ? "" : Executor?.Email ?? Issue.ExecutorIdentityUserId;
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        Executor = UsersIdentityDump?.FirstOrDefault(x => x.UserId == Issue.ExecutorIdentityUserId);
        editExecutorEmail = Executor?.Email ?? Issue.ExecutorIdentityUserId;
    }
}