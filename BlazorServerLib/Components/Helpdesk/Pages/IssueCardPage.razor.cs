////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.Pages;

/// <summary>
/// IssueCardPage
/// </summary>
public partial class IssueCardPage : BlazorBusyComponentBaseModel
{
    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Id
    /// </summary>
    [Parameter, EditorRequired]
    public int Id { get; set; }


    UserInfoModel CurrentUser { get; set; } = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<UserInfoModel?> user = await UsersProfilesRepo.FindByIdAsync();
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(user.Messages);
        CurrentUser = user.Response ?? throw new Exception();
    }
}