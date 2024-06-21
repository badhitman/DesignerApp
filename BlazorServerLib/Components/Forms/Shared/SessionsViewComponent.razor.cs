using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using BlazorWebLib.Components.Forms.Pages;

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

    /// <inheritdoc/>
    [CascadingParameter,EditorRequired]
    public required FormsPage CurrentMainProject { get; set; }

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
    protected string CreateSessionButtonTitle
    {
        get
        {
            if (CurrentMainProject is null)
                return "Не выбран основной/рабочий проект";

            if (SelectedQuestionnaireId < 1)
                return "Укажите анкету";

            return "Создать новую ссылку";
        }
    }
    //CurrentMainProject is null

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
            SnackbarRepo.Add($"rest.Content.Sessions is null. error B1F8BCC4-952B-4C5E-B573-6FA5AD7F3A8A", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
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
        TResponseModel<ConstructorFormSessionModelDB> rest = await FormsRepo.GetSessionQuestionnaire(session.Id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка E42D6754-5044-4D2E-BB8B-549CA385CCC2 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        StateHasChanged();

        DialogParameters<EditSessionDialogComponent> parameters = new()
        {
            { x => x.Session, rest.Response }
        };
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult result = await DialogServiceRepo.Show<EditSessionDialogComponent>($"Редактирование сессии. Опрос/анкета: '{rest.Response?.Owner?.Name}'", parameters, options).Result;
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
        TResponseModel<ConstructorFormSessionModelDB> rest = await FormsRepo.UpdateOrCreateSessionQuestionnaire(req);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        if (rest.Response is null)
        {
            SnackbarRepo.Add($"rest.Content.SessionQuestionnaire is null. error 9B2E03C0-0434-4F1A-B4E9-7020575DBDDF", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (sessions.Count != 0)
            sessions.Insert(0, rest.Response);
        else
            sessions.Add(rest.Response);

        SelectedQuestionnaireId = 0;
        NameSessionForCreate = null;

        if (table is not null)
            await table.ReloadServerData();
    }

    async Task RestUpdate()
    {
        if (CurrentMainProject.MainProject is null)
            throw new Exception("Не выбран основной/используемый проект");

        IsBusyProgress = true;
        ConstructorFormsQuestionnairesPaginationResponseModel rest = await FormsRepo.RequestQuestionnaires(new() { PageNum = 0, PageSize = 1000 }, CurrentMainProject.MainProject.Id);
        IsBusyProgress = false;

        if (rest.Questionnaires is null)
        {
            SnackbarRepo.Add($"rest.Content.Questionnaires is null. error 0A875193-08AA-4678-824D-213BCE33080F", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
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