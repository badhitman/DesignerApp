////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OrderDocumentComponent
/// </summary>
public partial class OrderDocumentComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    AuthenticationStateProvider AuthRepo { get; set; } = default!;

    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Заказ
    /// </summary>
    [Parameter, EditorRequired]
    public required int DocumentId { get; set; }


    UserInfoMainModel user = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await AuthRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();
    }
}