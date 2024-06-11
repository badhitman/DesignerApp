using BlazorLib;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Edit session dialog
/// </summary>
public partial class EditSessionDialogComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected NavigationManager NavManagerRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IJSRuntime JsRuntimeRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required ConstructorFormSessionModelDB Session { get; set; }

    string UrlSession => $"{NavManagerRepo.BaseUri}{ControllersAndActionsStatic.QUESTIONNAIRE_ACTION_NAME}/{ControllersAndActionsStatic.SESSION_ACTION_NAME}/{Session.SessionToken}";

    /// <inheritdoc/>
    protected async Task ClipboardCopyHandle()
    {
        await JsRuntimeRepo.InvokeVoidAsync("clipboardCopy.copyText", UrlSession);
        SnackbarRepo.Add($"Ссылка {Session.SessionToken} скопирована в буфер обмена", Severity.Info, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
    }

    /// <inheritdoc/>
    protected async Task DestroyLinkAccess()
    {
        session_origin.SessionToken = null;
        await SaveForm();
        SnackbarRepo.Add($"Ссылка аннулирована", Severity.Info, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
    }

    /// <inheritdoc/>
    protected async Task ReGenerate()
    {
        session_origin.SessionToken = Guid.Empty.ToString();
        await SaveForm();
        SnackbarRepo.Add($"Ссылка перевыпущена", Severity.Info, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
    }

    /// <inheritdoc/>
    protected bool IsEdited => Session.Name != session_origin.Name || Session.SessionStatus != session_origin.SessionStatus || (Session.Description ?? "") != (session_origin.Description ?? "") || Session.EmailsNotifications != session_origin.EmailsNotifications || Session.ShowDescriptionAsStartPage != session_origin.ShowDescriptionAsStartPage || Session.DeadlineDate != session_origin.DeadlineDate;

    /// <inheritdoc/>
    protected InputRichTextComponent? _currentTemplateInputRichText;

    ConstructorFormSessionModelDB session_origin = default!;

    /// <inheritdoc/>
    protected string GetHelperTextForDeadlineDate
    {
        get
        {
            string _res = "Срок действия токена";
            if (!session_origin.DeadlineDate.HasValue || Session.SessionStatus >= SessionsStatusesEnum.Sended)
                return _res;

            if (session_origin.DeadlineDate.Value < DateTime.Now)
                _res += $": просрочен [{(DateTime.Now - session_origin.DeadlineDate.Value):%d}] дней";
            else if (session_origin.DeadlineDate.Value > DateTime.Now)
                _res += $": осталось {(session_origin.DeadlineDate.Value - DateTime.Now):%d} дней";

            return _res;
        }
    }

    /// <inheritdoc/>
    protected void Close() => MudDialog.Close(DialogResult.Ok(Session));

    async Task ResetForm()
    {
        session_origin = new()
        {
            Id = Session.Id,
            CreatedAt = Session.CreatedAt,
            CreatorEmail = Session.CreatorEmail,
            DeadlineDate = Session.DeadlineDate,
            Description = Session.Description,
            Editors = Session.Editors,
            EmailsNotifications = Session.EmailsNotifications,
            LastQuestionnaireUpdateActivity = Session.LastQuestionnaireUpdateActivity,
            Name = Session.Name,
            OwnerId = Session.OwnerId,
            SessionStatus = Session.SessionStatus,
            SessionToken = Session.SessionToken,
            ShowDescriptionAsStartPage = Session.ShowDescriptionAsStartPage
        };

        if (_currentTemplateInputRichText is not null)
            await JsRuntimeRepo.InvokeVoidAsync("CKEditorInterop.setValue", _currentTemplateInputRichText.UID, session_origin.Description);

        StateHasChanged();
    }

    async Task SaveForm()
    {
        IsBusyProgress = true;
        TResponseModel<ConstructorFormSessionModelDB> rest = await FormsRepo.UpdateOrCreateSessionQuestionnaire(session_origin);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{64DC99E0-6350-4B82-A260-2CE6E791542D}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.Response is null)
        {
            SnackbarRepo.Add($"Ошибка {{B86A9186-FF44-473B-A478-0098E4B487B0}} rest.Content.Form is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        Session = rest.Response;
        await ResetForm();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ResetForm();
        base.OnInitialized();
    }
}