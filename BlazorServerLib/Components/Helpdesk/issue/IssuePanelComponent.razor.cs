////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using SharedLib;
using BlazorLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// Панель (справа)
/// </summary>
public partial class IssuePanelComponent : IssueWrapBaseModel
{
    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;

    UserInfoMainModel user = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();
    }
}