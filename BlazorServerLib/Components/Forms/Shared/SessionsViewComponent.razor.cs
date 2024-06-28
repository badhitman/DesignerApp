////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Sessions view
/// </summary>
public partial class SessionsViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IDialogService DialogServiceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UsersProfilesRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }


    IEnumerable<DocumentSchemeConstructorModelDB> QuestionnairesAll = [];

    int _selectedDocumentSchemeId;
    int SelectedDocumentSchemeId
    {
        get => _selectedDocumentSchemeId;
        set
        {
            _selectedDocumentSchemeId = value;
            if (table is not null)
                InvokeAsync(table.ReloadServerData);
        }
    }

    string? NameSessionForCreate { get; set; }
    string? searchString = null;

    /// <inheritdoc/>
    protected string CreateSessionButtonTitle
    {
        get
        {
            if (ParentFormsPage is null)
                return "Не выбран основной/рабочий проект";

            if (SelectedDocumentSchemeId < 1)
                return "Укажите анкету";

            return "Создать новую ссылку";
        }
    }

    protected private async Task<TableData<SessionOfDocumentDataModelDB>> ServerReload(TableState state)
    {
        if (string.IsNullOrWhiteSpace(CurrentUser.UserId))
            throw new Exception("Не определён текущий пользователь");

        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не установлен основной проект");

        RequestSessionsQuestionnairesRequestPaginationModel req = new()
        {
            PageNum = state.Page,
            PageSize = state.PageSize,
            SimpleRequest = searchString,
            DocumentSchemeId = SelectedDocumentSchemeId,
            FilterUserId = CurrentUser.UserId,
            ProjectId = ParentFormsPage.MainProject.Id
        };
        IsBusyProgress = true;
        ConstructorFormsSessionsPaginationResponseModel rest = await FormsRepo.RequestSessionsQuestionnaires(req);
        IsBusyProgress = false;

        if (rest.Sessions is null)
        {
            SnackbarRepo.Add($"rest.Content.Sessions is null. error B1F8BCC4-952B-4C5E-B573-6FA5AD7F3A8A", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return new TableData<SessionOfDocumentDataModelDB>() { TotalItems = totalItems, Items = sessions };
        }

        totalItems = rest.TotalRowsCount;
        sessions = new(rest.Sessions);

        return new TableData<SessionOfDocumentDataModelDB>() { TotalItems = totalItems, Items = sessions };
    }

    /// <inheritdoc/>
    protected async Task EditSession(SessionOfDocumentDataModelDB session)
    {
        IsBusyProgress = true;
        TResponseModel<SessionOfDocumentDataModelDB> rest = await FormsRepo.GetSessionQuestionnaire(session.Id);
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
    protected MudTable<SessionOfDocumentDataModelDB>? table;

    /// <inheritdoc/>
    protected int totalItems;
    List<SessionOfDocumentDataModelDB> sessions = [];

    /// <inheritdoc/>
    protected async Task CreateNewSession()
    {
        if (string.IsNullOrWhiteSpace(NameSessionForCreate))
        {
            SnackbarRepo.Error("Укажите имя ссылки");
            return;
        }

        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран основной/текущий проект");

        TResponseModel<UserInfoModel?> current_user = await UsersProfilesRepo.FindByIdAsync();

        if (current_user.Response?.UserId is null)
            throw new Exception("Ошибка установки автора ссылки");

        SessionOfDocumentDataModelDB req = new()
        {
            Name = NameSessionForCreate,
            OwnerId = SelectedDocumentSchemeId,
            AuthorUser = current_user.Response.UserId,
            ProjectId = ParentFormsPage.MainProject.Id
        };
        IsBusyProgress = true;
        TResponseModel<SessionOfDocumentDataModelDB> rest = await FormsRepo.UpdateOrCreateSessionQuestionnaire(req);
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

        SelectedDocumentSchemeId = 0;
        NameSessionForCreate = null;

        if (table is not null)
            await table.ReloadServerData();
    }

    async Task RestUpdate()
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран основной/используемый проект");

        IsBusyProgress = true;
        ConstructorFormsQuestionnairesPaginationResponseModel rest = await FormsRepo.RequestDocumentsSchemes(new() { PageNum = 0, PageSize = 1000 }, ParentFormsPage.MainProject.Id);
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