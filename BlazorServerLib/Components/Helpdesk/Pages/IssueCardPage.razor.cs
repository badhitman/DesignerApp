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
    IssueHelpdeskModelDB? IssueSource { get; set; }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<UserInfoModel?> user = await UsersProfilesRepo.FindByIdAsync();
        SnackbarRepo.ShowMessagesResponse(user.Messages);
        CurrentUser = user.Response ?? throw new Exception();
        TResponseModel<IssueHelpdeskModelDB?> issue_res = await HelpdeskRepo.IssueRead(new IssueReadRequestModel() { IssueId = Id, UserIdentityId = CurrentUser.UserId });
        SnackbarRepo.ShowMessagesResponse(issue_res.Messages);
        IssueSource = issue_res.Response;
        IsBusyProgress = false;
    }
}