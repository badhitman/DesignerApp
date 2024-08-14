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
    UserInfoModel? Author => UsersIdentityDump?.FirstOrDefault(x => x.UserId == Issue.AuthorIdentityUserId);
}