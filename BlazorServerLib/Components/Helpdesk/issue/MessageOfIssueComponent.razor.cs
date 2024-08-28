////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;
using System.ComponentModel;

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

    bool IsInitDelete;

     Task TryDelete()
    {
        if(!IsInitDelete)
        {
            IsInitDelete = true;
            //return;
        }
        return Task.CompletedTask;
    }

    string? textMessage { get; set; }

    bool canSave => (IsCreatingNewMessage || textMessage != Message?.MessageText) && !string.IsNullOrEmpty(textMessage);

    MarkupString DescriptionHtml => (MarkupString)(Message?.MessageText ?? "");

    /// <summary>
    /// типы отношений к сообщению
    /// </summary>
    enum AuthorsTypesEnum
    {
        /// <summary>
        /// Обычный: не является автором или исполнителем обращения
        /// </summary>
        [Description("secondary")]
        Simple,

        /// <summary>
        /// Системное (сообщение от имени системы)
        /// </summary>
        [Description("success-emphasis")]
        System,

        /// <summary>
        /// Сообщение ваше (вы автор сообщения)
        /// </summary>
        [Description("primary-emphasis")]
        My,

        /// <summary>
        /// Исполнитель
        /// </summary>
        [Description("info-emphasis")]
        Executor,
    }

    AuthorsTypesEnum _currentType = AuthorsTypesEnum.Simple;

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
                    Id = Message?.Id ?? 0
                }
            });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        ParentListIssues.AddingNewMessage = false;
        IsEditMode = false;
        await ParentListIssues.ReloadMessages();
    }

    void Cancel()
    {
        textMessage = Message?.MessageText;
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

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (Message is null || Message.Id < 1)
            IsEditMode = true;

        textMessage = Message?.MessageText;

        if(Message?.AuthorUserId == GlobalStaticConstants.Roles.System)
            _currentType = AuthorsTypesEnum.System;
        else if (Message?.AuthorUserId == CurrentUser.UserId)
            _currentType = AuthorsTypesEnum.My;
        else if (Message?.AuthorUserId == Issue.ExecutorIdentityUserId)
            _currentType = AuthorsTypesEnum.Executor;
    }
}