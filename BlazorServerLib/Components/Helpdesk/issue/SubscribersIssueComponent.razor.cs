////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// Участники диалога
/// </summary>
public partial class SubscribersIssueComponent : IssueWrapBaseModel
{
    bool CanSubscribe=> Issue.Subscribers?.Any(x => x.AuthorIdentityUserId == CurrentUser.UserId) != true;
}