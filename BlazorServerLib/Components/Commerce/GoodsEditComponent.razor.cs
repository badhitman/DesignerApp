////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// GoodsEditComponent
/// </summary>
public partial class GoodsEditComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// GoodsId
    /// </summary>
    [Parameter, EditorRequired]
    public required int GoodsId { get; set; }


    UserInfoMainModel user = default!;
    GoodsModelDB? CurrentGoods;
    GoodsModelDB? editGoods;

    bool CanSave => CurrentGoods is not null && (CurrentGoods.Name != editGoods?.Name || CurrentGoods.BaseUnit != editGoods?.BaseUnit);

    async Task SaveGoods()
    {
        if (editGoods is null)
            throw new ArgumentNullException(nameof(editGoods));

        IsBusyProgress = true;
        TResponseModel<int> res = await CommerceRepo.GoodUpdateReceive(editGoods);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success())
            CurrentGoods = GlobalTools.CreateDeepCopy(editGoods);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();
        TResponseModel<GoodsModelDB[]> res = await CommerceRepo.GoodsRead([GoodsId]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success() && res.Response is not null && res.Response.Length != 0)
        {
            CurrentGoods = res.Response.Single();
            editGoods = GlobalTools.CreateDeepCopy(CurrentGoods);
        }
    }
}