////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.issue;

/// <summary>
/// MessageOfIssueComponent
/// </summary>
public partial class MessageOfIssueComponent : IssueWrapBaseModel
{
    /// <summary>
    /// Message 
    /// </summary>
    [Parameter]
    public IssueMessageHelpdeskModelDB? Message { get; set; }


    /// <summary>
    /// ParentListIssues
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required IssueMessagesComponent ParentListIssues { get; set; }


    bool IsCreatingNewMessage => Message is null || Message.Id < 1;

    bool IsEditMode;

    string? textMessage { get; set; }

    bool canSave => IsCreatingNewMessage && !string.IsNullOrEmpty(textMessage);

    async Task SaveMessage()
    {
        if (string.IsNullOrWhiteSpace(textMessage))
            throw new ArgumentNullException(nameof(textMessage));

        IsBusyProgress = true;
        TResponseModel<int> rest = await HelpdeskRepo
            .MessageCreateOrUpdate(new()
            {
                SenderActionUserId = CurrentUser.UserId,
                Payload = new()
                {
                    MessageText = textMessage,
                    IssueId = Issue.Id,
                }
            });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        ParentListIssues.AddingNewMessage = false;
        await ParentListIssues.ReloadMessages();
    }

    void Cancel()
    {
        if (Message is null || Message.Id < 1)
        {
            ParentListIssues.AddingNewMessage = false;
            ParentListIssues.StateHasChangedCall();
        }
        else
        {
            IsEditMode = false;
        }
    }

    MarkupString DescriptionHtml => (MarkupString)(Message?.MessageText ?? "");

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (Message is null || Message.Id < 1)
            IsEditMode = true;

        textMessage = Message?.MessageText;
    }
}