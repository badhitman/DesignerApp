////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// Участники диалога
/// </summary>
public partial class StatusIssueComponent : IssueWrapBaseModel
{
    HelpdeskIssueStepsEnum IssueStep { get; set; }

    List<HelpdeskIssueStepsEnum> Steps()
    {
        List<HelpdeskIssueStepsEnum> res = [];

        if (CurrentUser.IsAdmin || CurrentUser.UserId == Issue.ExecutorIdentityUserId || CurrentUser.Roles?.Contains(GlobalStaticConstants.Roles.HelpDeskTelegramBotManager) == true)
            res.AddRange(Enum.GetValues<HelpdeskIssueStepsEnum>());
        else
        {
            switch (Issue.StepIssue)
            {
                case HelpdeskIssueStepsEnum.Created or HelpdeskIssueStepsEnum.Reopen or HelpdeskIssueStepsEnum.Pause or HelpdeskIssueStepsEnum.Progress or HelpdeskIssueStepsEnum.Check:
                    res.AddRange([Issue.StepIssue, HelpdeskIssueStepsEnum.Done, HelpdeskIssueStepsEnum.Canceled]);
                    break;
                case HelpdeskIssueStepsEnum.Done:
                    res.AddRange([HelpdeskIssueStepsEnum.Done, HelpdeskIssueStepsEnum.Reopen]);
                    break;
                case HelpdeskIssueStepsEnum.Canceled:
                    res.AddRange([HelpdeskIssueStepsEnum.Canceled, HelpdeskIssueStepsEnum.Reopen]);
                    break;
            }
        }

        return res;
    }

    async Task SaveChange()
    {
        IsBusyProgress = true;
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
    protected override void OnInitialized()
    {
        IssueStep = Issue.StepIssue;
        base.OnInitialized();
    }
}