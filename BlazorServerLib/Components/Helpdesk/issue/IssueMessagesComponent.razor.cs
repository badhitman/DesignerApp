////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;
using System.Net.Http;
using static MudBlazor.CategoryTypes;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// IssueMessagesComponent
/// </summary>
public partial class IssueMessagesComponent : IssueWrapBaseModel
{
    [Inject]
    IWebRemoteTransmissionService WebRemoteRepo { get; set; } = default!;


    private IEnumerable<IssueMessageHelpdeskModelDB> Elements = [];

    MudTable<IssueMessageHelpdeskModelDB>? tableRef;

    /// <summary>
    /// Добавляется новое сообщение
    /// </summary>
    public bool AddingNewMessage = false;

    private string _searchStringQuery = "";
    private string searchStringQuery
    {
        get => _searchStringQuery;
        set
        {
            _searchStringQuery = value;
            if (tableRef is not null)
                InvokeAsync(tableRef.ReloadServerData);
        }
    }

    IssueMessageHelpdeskModelDB[]? messages;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private Task<TableData<IssueMessageHelpdeskModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        if (messages is null)
            return Task.FromResult(new TableData<IssueMessageHelpdeskModelDB>() { TotalItems = 0, Items = [] });

        IssueMessageHelpdeskModelDB[] _messages = string.IsNullOrWhiteSpace(searchStringQuery)
            ? messages
            : [.. messages.Where(x => x.MessageText.Contains(searchStringQuery, StringComparison.OrdinalIgnoreCase))];

        return Task.FromResult(new TableData<IssueMessageHelpdeskModelDB>() { TotalItems = _messages.Length, Items = _messages.OrderBy(x => x.Id).Skip(state.PageSize * state.Page).Take(state.PageSize) });
    }

    /// <summary>
    /// ReloadMessages
    /// </summary>
    public async Task ReloadMessages()
    {
        await SetBusy();
        TResponseModel<IssueMessageHelpdeskModelDB[]> messages_rest = await HelpdeskRepo.MessagesList(new()
        {
            Payload = Issue.Id,
            SenderActionUserId = CurrentUserSession!.UserId,
        });
        IsBusyProgress = false;
        messages = messages_rest.Response;
        SnackbarRepo.ShowMessagesResponse(messages_rest.Messages);
        if (!messages_rest.Success() || messages_rest.Response is null)
            return;

        Issue.Messages = [.. messages_rest.Response];

        string[] users_for_adding = Issue
            .Messages
            .Where(x => x.AuthorUserId != GlobalStaticConstants.Roles.System && !UsersIdentityDump.Any(y => y.UserId == x.AuthorUserId))
            .Select(x => x.AuthorUserId)
            .ToArray();

        if (users_for_adding.Length != 0)
        {
            TResponseModel<UserInfoModel[]?> users_data_identity = await WebRemoteRepo.GetUsersIdentity([.. users_for_adding.Distinct()]);
            SnackbarRepo.ShowMessagesResponse(users_data_identity.Messages);
            if (users_data_identity.Response is not null && users_data_identity.Response.Length != 0)
                UsersIdentityDump.AddRange(users_data_identity.Response);
        }
        if (tableRef is not null)
            await tableRef.ReloadServerData();
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReloadMessages();
    }
}