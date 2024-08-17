////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// Участники диалога
/// </summary>
public partial class StatusIssueComponent : IssueWrapBaseModel
{

    HelpdeskIssueStepsEnum issueStep { get; set; }

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

    string ChangeAbout()
    {
        switch (Issue.StepIssue)
        {
            case HelpdeskIssueStepsEnum.Created:

                break;
            case HelpdeskIssueStepsEnum.Reopen:

                break;
            case HelpdeskIssueStepsEnum.Pause:

                break;
            case HelpdeskIssueStepsEnum.Progress:

                break;
            case HelpdeskIssueStepsEnum.Check:

                break;
            case HelpdeskIssueStepsEnum.Done:

                break;
            case HelpdeskIssueStepsEnum.Canceled:

                break;
        }

        return "";
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        issueStep = Issue.StepIssue;
        base.OnInitialized();
    }
}