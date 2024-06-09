using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class SessionsViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<SessionsViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected IDialogService _dialog_service { get; set; } = default!;

    [Inject]
    protected IJSRuntime _js_runtime { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;


    IEnumerable<ConstructorFormQuestionnaireModelDB> QuestionnairesAll = Enumerable.Empty<ConstructorFormQuestionnaireModelDB>();

    int _selectedQuestionnaireId;
    int selectedQuestionnaireId
    {
        get => _selectedQuestionnaireId;
        set
        {
            _selectedQuestionnaireId = value;
            if (table is not null)
                InvokeAsync(async () => await table.ReloadServerData());
        }
    }

    string? nameSessionForCreate { get; set; }
    string? searchString = null;

    protected string CreateSessionButtonTitle => selectedQuestionnaireId < 1 ? "Укажите анкету" : "Создать новую ссылку";

    protected private async Task<TableData<ConstructorFormSessionModelDB>> ServerReload(TableState state)
    {
        RequestSessionsQuestionnairesRequestPaginationModel req = new()
        {
            PageNum = state.Page,
            PageSize = state.PageSize,
            SimpleRequest = searchString,
            QuestionnaireId = selectedQuestionnaireId
        };
        IsBusyProgress = true;
        ConstructorFormsSessionsPaginationResponseModel rest = await _forms.RequestSessionsQuestionnaires(req);
        IsBusyProgress = false;

        if (rest.Sessions is null)
        {
            _snackbar.Add($"rest.Content.Sessions is null. error {{532E45D0-7584-42CE-B15F-E3E45DDE2E0E}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return new TableData<ConstructorFormSessionModelDB>() { TotalItems = totalItems, Items = sessions };
        }

        totalItems = rest.TotalRowsCount;
        sessions = new(rest.Sessions);

        return new TableData<ConstructorFormSessionModelDB>() { TotalItems = totalItems, Items = sessions };
    }

    protected async Task EditSession(ConstructorFormSessionModelDB session)
    {
        IsBusyProgress = true;
        FormSessionQuestionnairieResponseModel rest = await _forms.GetSessionQuestionnairie(session.Id);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{F0B514BB-C043-4733-A420-EC8A77C66252}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        StateHasChanged();

        DialogParameters<EditSessionDialogComponent> parameters = new()
        {
            { x => x.Session, rest.SessionQuestionnairie }
        };
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult result = await _dialog_service.Show<EditSessionDialogComponent>($"Редактирование сессии. Опрос/анкета: '{rest.SessionQuestionnairie?.Owner?.Name}'", parameters, options).Result;
        if (table is not null)
            await table.ReloadServerData();
    }

    protected async Task DeleteSession(int sesssion_id)
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await _forms.DeleteSessionQuestionnaire(sesssion_id);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        if (table is not null)
            await table.ReloadServerData();
    }

    protected async Task OnSearch(string text)
    {
        searchString = text;
        if (table is not null)
            await table.ReloadServerData();
    }

    protected MudTable<ConstructorFormSessionModelDB>? table;

    protected int totalItems;
    List<ConstructorFormSessionModelDB> sessions = [];

    protected async Task CreateNewSession()
    {
        ConstructorFormSessionModelDB req = new()
        {
            Name = nameSessionForCreate ?? "",
            SessionToken = Guid.NewGuid().ToString(),
            OwnerId = selectedQuestionnaireId
        };
        IsBusyProgress = true;
        FormSessionQuestionnairieResponseModel rest = await _forms.UpdateOrCreateSessionQuestionnairie(req);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        if (rest.SessionQuestionnairie is null)
        {
            _snackbar.Add($"rest.Content.SessionQuestionnairie is null. error {{3B473C9F-CCE2-4CAF-B39C-7286EA0AF3A7}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (sessions.Count != 0)
            sessions.Insert(0, rest.SessionQuestionnairie);
        else
            sessions.Add(rest.SessionQuestionnairie);

        selectedQuestionnaireId = 0;
        nameSessionForCreate = null;

        if (table is not null)
            await table.ReloadServerData();
    }

    async Task RestUpdate()
    {
        IsBusyProgress = true;
        ConstructorFormsQuestionnairiesPaginationResponseModel rest = await _forms.RequestQuestionnaires(new() { PageNum = 0, PageSize = 1000 });
        IsBusyProgress = false;

        if (rest.Questionnairies is null)
        {
            _snackbar.Add($"rest.Content.Questionnaires is null. error {{9183C745-5387-401E-8B5A-6D6B8D461FA7}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        QuestionnairesAll = rest.Questionnairies;
    }

    protected override async Task OnInitializedAsync()
    {
        await RestUpdate();
    }
}