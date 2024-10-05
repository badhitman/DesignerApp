////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using SharedLib;
using BlazorLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// AboutIssueComponent
/// </summary>
public partial class AboutIssueComponent : IssueWrapBaseModel
{
    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;


    UserInfoMainModel user = default!;
    bool IsShow;
    UserInfoModel? Author;


    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();
        Author = UsersIdentityDump?.FirstOrDefault(x => x.UserId == Issue.AuthorIdentityUserId);
    }
}