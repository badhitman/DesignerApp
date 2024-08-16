////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

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
        IsBusyProgress = true;
        IsBusyProgress = false;
        await Task.Delay(1000);
        throw new NotImplementedException();
    }

    void Cancel()
    {
        if (Message is null || Message.Id < 1)
        {
            ParentListIssues.AddingNewMessage = false;
            ParentListIssues.StateHasChangedCall();
        }
    }

    MarkupString DescriptionHtml => (MarkupString)(Message?.MessageText ?? "");

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (Message is null || Message.Id < 1)
            IsEditMode = true;
    }
}