using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class EditSessionDialogComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<EditSessionDialogComponent> _logger { get; set; } = default!;

    [Inject]
    protected NavigationManager _nav { get; set; } = default!;

    [Inject]
    protected IJSRuntime _js_runtime { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter, EditorRequired]
    public ConstructorFormSessionModelDB Session { get; set; } = default!;

    string _uri => $"{_nav.BaseUri}{ControllersAndActionsStatic.QUESTIONNAIRE_ACTION_NAME}/{ControllersAndActionsStatic.SESSION_ACTION_NAME}/{Session.SessionToken}";

    protected async Task ClipboardCopyHandle()
    {
        await _js_runtime.InvokeVoidAsync("clipboardCopy.copyText", _uri);
        _snackbar.Add($"Ссылка {Session.SessionToken} скопирована в буфер обмена", Severity.Info, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
    }

    protected async Task DestroyLinkAccess()
    {
        session_orign.SessionToken = null;
        await SaveForm();
        _snackbar.Add($"Ссылка аннулирована", Severity.Info, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
    }

    protected async Task ReGenerate()
    {
        session_orign.SessionToken = Guid.Empty.ToString();
        await SaveForm();
        _snackbar.Add($"Ссылка перевыпущена", Severity.Info, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
    }

    protected bool IsEdited => Session.Name != session_orign.Name || Session.SessionStatus != session_orign.SessionStatus || (Session.Description ?? "") != (session_orign.Description ?? "") || Session.EmailsNotifications != session_orign.EmailsNotifications || Session.ShowDescriptionAsStartPage != session_orign.ShowDescriptionAsStartPage || Session.DeadlineDate != session_orign.DeadlineDate;

    protected InputRichTextComponent? _currentTemplateInputRichText;

    ConstructorFormSessionModelDB session_orign = default!;

    protected string GetHelperTextForDeadlineDate
    {
        get
        {
            string _res = "Срок действия токена";
            if (!session_orign.DeadlineDate.HasValue || Session.SessionStatus >= SessionsStatusesEnum.Sended)
                return _res;

            if (session_orign.DeadlineDate.Value < DateTime.Now)
                _res += $": просрочен [{(DateTime.Now - session_orign.DeadlineDate.Value):%d}] дней";
            else if (session_orign.DeadlineDate.Value > DateTime.Now)
                _res += $": осталось {(session_orign.DeadlineDate.Value - DateTime.Now):%d} дней";

            return _res;
        }
    }

    protected void Close() => MudDialog.Close(DialogResult.Ok(Session));

    async Task ResetForm()
    {
        session_orign = new()
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
            await _js_runtime.InvokeVoidAsync("CKEditorInterop.setValue", _currentTemplateInputRichText.UID, session_orign.Description);

        StateHasChanged();
    }

    async Task SaveForm()
    {
        IsBusyProgress = true;
        FormSessionQuestionnaireResponseModel rest = await _forms.UpdateOrCreateSessionQuestionnaire(session_orign);
        IsBusyProgress = false;
        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{64DC99E0-6350-4B82-A260-2CE6E791542D}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.SessionQuestionnaire is null)
        {
            _snackbar.Add($"Ошибка {{B86A9186-FF44-473B-A478-0098E4B487B0}} rest.Content.Form is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        Session = rest.SessionQuestionnaire;
        await ResetForm();
    }

/// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ResetForm();
        base.OnInitialized();
    }
}