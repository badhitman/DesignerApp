using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Sessions view
/// </summary>
public partial class SessionsViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected IDialogService DialogServiceRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;


    IEnumerable<ConstructorFormQuestionnaireModelDB> QuestionnairesAll = [];

    int _selectedQuestionnaireId;
    int SelectedQuestionnaireId
    {
        get => _selectedQuestionnaireId;
        set
        {
            _selectedQuestionnaireId = value;
            if (table is not null)
                InvokeAsync(async () => await table.ReloadServerData());
        }
    }

    string? NameSessionForCreate { get; set; }
    string? searchString = null;

    /// <inheritdoc/>
    protected string CreateSessionButtonTitle => SelectedQuestionnaireId < 1 ? "Укажите анкету" : "Создать новую ссылку";

    protected private async Task<TableData<ConstructorFormSessionModelDB>> ServerReload(TableState state)
    {
        RequestSessionsQuestionnairesRequestPaginationModel req = new()
        {
            PageNum = state.Page,
            PageSize = state.PageSize,
            SimpleRequest = searchString,
            QuestionnaireId = SelectedQuestionnaireId
        };
        IsBusyProgress = true;
        ConstructorFormsSessionsPaginationResponseModel rest = await FormsRepo.RequestSessionsQuestionnaires(req);
        IsBusyProgress = false;

        if (rest.Sessions is null)
        {
            SnackbarRepo.Add($"rest.Content.Sessions is null. error {{532E45D0-7584-42CE-B15F-E3E45DDE2E0E}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return new TableData<ConstructorFormSessionModelDB>() { TotalItems = totalItems, Items = sessions };
        }

        totalItems = rest.TotalRowsCount;
        sessions = new(rest.Sessions);

        return new TableData<ConstructorFormSessionModelDB>() { TotalItems = totalItems, Items = sessions };
    }

    /// <inheritdoc/>
    protected async Task EditSession(ConstructorFormSessionModelDB session)
    {
        IsBusyProgress = true;
        FormSessionQuestionnaireResponseModel rest = await FormsRepo.GetSessionQuestionnaire(session.Id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{F0B514BB-C043-4733-A420-EC8A77C66252}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        StateHasChanged();

        DialogParameters<EditSessionDialogComponent> parameters = new()
        {
            { x => x.Session, rest.SessionQuestionnaire }
        };
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult result = await DialogServiceRepo.Show<EditSessionDialogComponent>($"Редактирование сессии. Опрос/анкета: '{rest.SessionQuestionnaire?.Owner?.Name}'", parameters, options).Result;
        if (table is not null)
            await table.ReloadServerData();
    }

    /// <inheritdoc/>
    protected async Task DeleteSession(int session_id)
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.DeleteSessionQuestionnaire(session_id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        if (table is not null)
            await table.ReloadServerData();
    }

    /// <inheritdoc/>
    protected async Task OnSearch(string text)
    {
        searchString = text;
        if (table is not null)
            await table.ReloadServerData();
    }

    /// <inheritdoc/>
    protected MudTable<ConstructorFormSessionModelDB>? table;

    /// <inheritdoc/>
    protected int totalItems;
    List<ConstructorFormSessionModelDB> sessions = [];

    /// <inheritdoc/>
    protected async Task CreateNewSession()
    {
        ConstructorFormSessionModelDB req = new()
        {
            Name = NameSessionForCreate ?? "",
            SessionToken = Guid.NewGuid().ToString(),
            OwnerId = SelectedQuestionnaireId
        };
        IsBusyProgress = true;
        FormSessionQuestionnaireResponseModel rest = await FormsRepo.UpdateOrCreateSessionQuestionnaire(req);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        if (rest.SessionQuestionnaire is null)
        {
            SnackbarRepo.Add($"rest.Content.SessionQuestionnaire is null. error {{3B473C9F-CCE2-4CAF-B39C-7286EA0AF3A7}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (sessions.Count != 0)
            sessions.Insert(0, rest.SessionQuestionnaire);
        else
            sessions.Add(rest.SessionQuestionnaire);

        SelectedQuestionnaireId = 0;
        NameSessionForCreate = null;

        if (table is not null)
            await table.ReloadServerData();
    }

    async Task RestUpdate()
    {
        IsBusyProgress = true;
        ConstructorFormsQuestionnairesPaginationResponseModel rest = await FormsRepo.RequestQuestionnaires(new() { PageNum = 0, PageSize = 1000 });
        IsBusyProgress = false;

        if (rest.Questionnaires is null)
        {
            SnackbarRepo.Add($"rest.Content.Questionnaires is null. error {{9183C745-5387-401E-8B5A-6D6B8D461FA7}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        QuestionnairesAll = rest.Questionnaires;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await RestUpdate();
    }
}