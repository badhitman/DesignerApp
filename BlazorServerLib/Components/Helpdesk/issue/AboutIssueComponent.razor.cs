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
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Author = UsersIdentityDump?.FirstOrDefault(x => x.UserId == Issue.AuthorIdentityUserId);
    }
}