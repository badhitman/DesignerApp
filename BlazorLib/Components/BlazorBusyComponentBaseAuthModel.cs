////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SharedLib;

namespace BlazorLib;

/// <summary>
/// BlazorBusyComponentBaseAuthModel
/// </summary>
public abstract class BlazorBusyComponentBaseAuthModel : BlazorBusyComponentBaseModel
{
    [Inject]
    AuthenticationStateProvider AuthRepo { get; set; } = default!;

    /// <summary>
    /// Текущий пользователь (сессия)
    /// </summary>
    public UserInfoMainModel? CurrentUserSession { get; private set; }


    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await AuthRepo.GetAuthenticationStateAsync();
        CurrentUserSession = state.User.ReadCurrentUserInfo();
    }
}
