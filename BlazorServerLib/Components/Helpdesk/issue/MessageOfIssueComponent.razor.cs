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


    string images_upload_url = default!;
    Dictionary<string, object> editorConf = default!;


    bool IsCreatingNewMessage => Message is null || Message.Id < 1;

    bool IsEditMode;

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

        await SetBusy();
        TResponseModel<int> rest = await HelpdeskRepo
            .MessageCreateOrUpdate(new()
            {
                SenderActionUserId = CurrentUserSession!.UserId,
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
    protected override async void OnInitialized()
    {
        await base.OnInitializedAsync();

        /*
         <FilesContextViewComponent ApplicationsNames="@([GlobalStaticConstants.Routes.ISSUE_CONTROLLER_NAME])"
                                                                               PropertyName="@GlobalStaticConstants.Routes.ATTACHMENT_CONTROLLER_NAME"
                                                                               PrefixPropertyName="@GlobalStaticConstants.Routes.USER_CONTROLLER_NAME"
                                                                               OwnerPrimaryKey="Id"
                                                                               Title="Файлы"
                                                                               ManageMode="true" />
         */

        images_upload_url = $"{GlobalStaticConstants.TinyMCEditorUploadImage}{GlobalStaticConstants.Routes.ISSUE_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.MESSAGE_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.IMAGE_ACTION_NAME}?{nameof(StorageMetadataModel.PrefixPropertyName)}={Message?.Id}&{nameof(StorageMetadataModel.OwnerPrimaryKey)}={Issue.Id}";
        editorConf = GlobalStaticConstants.TinyMCEditorConf(images_upload_url);

        if (Message is null || Message.Id < 1)
            IsEditMode = true;

        textMessage = Message?.MessageText;

        if (Message?.AuthorUserId == GlobalStaticConstants.Roles.System)
            _currentType = AuthorsTypesEnum.System;
        else if (Message?.AuthorUserId == CurrentUserSession!.UserId)
            _currentType = AuthorsTypesEnum.My;
        else if (Message?.AuthorUserId == Issue.ExecutorIdentityUserId)
            _currentType = AuthorsTypesEnum.Executor;
    }
}