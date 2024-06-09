using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class QuestionnairesViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<QuestionnairesViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected IDialogService _dialog_service { get; set; } = default!;

    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;


    MudTable<ConstructorFormQuestionnaireModelDB>? table;
    string? searchString;
    ConstructorFormsQuestionnairesPaginationResponseModel data = new() { Questionnaires = Enumerable.Empty<ConstructorFormQuestionnaireModelDB>() };

    protected static MarkupString Descr(string? html) => (MarkupString)(html ?? "");

    protected async Task DeleteQuestionnaire(int questionnaire_id)
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.DeleteQuestionnaire(questionnaire_id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{0D9D887E-A52D-49FF-8648-61E59F7D2DAA}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (table is not null)
            await table.ReloadServerData();
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    protected async Task<TableData<ConstructorFormQuestionnaireModelDB>> ServerReload(TableState state)
    {
        SimplePaginationRequestModel req = new();
        IsBusyProgress = true;
        data = await FormsRepo.RequestQuestionnaires(req);
        IsBusyProgress = false;

        if (data.Questionnaires is null)
        {
            SnackbarRepo.Add($"rest.Content.Questionnaires is null. error {{D03EAEDB-1430-41A0-95EA-3C2344CA0102}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return new TableData<ConstructorFormQuestionnaireModelDB>() { TotalItems = data.TotalRowsCount, Items = data.Questionnaires };
        }

        return new TableData<ConstructorFormQuestionnaireModelDB>() { TotalItems = data.TotalRowsCount, Items = data.Questionnaires };
    }

    protected async Task QuestionnaireOpenDialog(ConstructorFormQuestionnaireModelDB? questionnaire = null)
    {
        questionnaire ??= (ConstructorFormQuestionnaireModelDB)EntryDescriptionModel.Build("");
        DialogParameters<EditQuestionnaireDialogComponent> parameters = new();
        parameters.Add(x => x.Questionnaire, questionnaire);

        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult result = await _dialog_service.Show<EditQuestionnaireDialogComponent>(questionnaire.Id < 1 ? "Создание новой анкеты/опроса" : $"Редактирование анкеты/опроса #{questionnaire.Id}", parameters, options).Result;
        if (table is not null)
            await table.ReloadServerData();
    }

    protected async Task OnSearch(string text)
    {
        searchString = text;
        if (table is not null)
            await table.ReloadServerData();
    }
}