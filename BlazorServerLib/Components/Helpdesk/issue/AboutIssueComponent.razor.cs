////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// AboutIssueComponent
/// </summary>
public partial class AboutIssueComponent : IssueWrapBaseModel
{
    bool IsShow;

    UserInfoModel? Author;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        Author = UsersIdentityDump?.FirstOrDefault(x => x.UserId == Issue.AuthorIdentityUserId);
    }
}