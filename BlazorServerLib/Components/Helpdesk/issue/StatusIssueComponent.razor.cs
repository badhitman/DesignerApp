////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// Участники диалога
/// </summary>
public partial class StatusIssueComponent : IssueWrapBaseModel
{
    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;


    UserInfoMainModel user = default!;

    StatusesDocumentsEnum IssueStep { get; set; }

    List<StatusesDocumentsEnum> Steps()
    {
        List<StatusesDocumentsEnum> res = [];

        if (CurrentUser.IsAdmin || CurrentUser.UserId == Issue.ExecutorIdentityUserId || CurrentUser.Roles?.Contains(GlobalStaticConstants.Roles.HelpDeskTelegramBotManager) == true)
            res.AddRange(Enum.GetValues<StatusesDocumentsEnum>());
        else
        {
            switch (Issue.StepIssue)
            {
                case StatusesDocumentsEnum.Created or StatusesDocumentsEnum.Reopen or StatusesDocumentsEnum.Pause or StatusesDocumentsEnum.Progress or StatusesDocumentsEnum.Check:
                    res.AddRange([Issue.StepIssue, StatusesDocumentsEnum.Done, StatusesDocumentsEnum.Canceled]);
                    break;
                case StatusesDocumentsEnum.Done:
                    res.AddRange([StatusesDocumentsEnum.Done, StatusesDocumentsEnum.Reopen]);
                    break;
                case StatusesDocumentsEnum.Canceled:
                    res.AddRange([StatusesDocumentsEnum.Canceled, StatusesDocumentsEnum.Reopen]);
                    break;
            }
        }

        return res;
    }

    async Task SaveChange()
    {
        SetBusy();
        
        TResponseModel<bool> res = await HelpdeskRepo
            .StatusChange(new()
            {
                SenderActionUserId = CurrentUser.UserId,
                Payload = new()
                {
                    IssueId = Issue.Id,
                    Step = IssueStep
                }
            });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (!res.Success())
            return;

        //Issue.StepIssue = IssueStep;
        NavRepo.ReloadPage();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IssueStep = Issue.StepIssue;
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();
    }
}