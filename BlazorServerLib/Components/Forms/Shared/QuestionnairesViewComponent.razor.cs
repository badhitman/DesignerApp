﻿using BlazorLib;
using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Questionnaires view
/// </summary>
public partial class QuestionnairesViewComponent : BlazorBusyComponentBaseModel
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
    [CascadingParameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }

    MudTable<ConstructorFormQuestionnaireModelDB>? table;

    /// <inheritdoc/>
    protected string? searchString;
    ConstructorFormsQuestionnairesPaginationResponseModel data = new() { Questionnaires = Enumerable.Empty<ConstructorFormQuestionnaireModelDB>() };

    /// <inheritdoc/>
    protected static MarkupString Descr(string? html) => (MarkupString)(html ?? "");

    /// <inheritdoc/>
    protected async Task DeleteQuestionnaire(int questionnaire_id)
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.DeleteQuestionnaire(questionnaire_id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка F1AADB25-31FF-4305-90A9-4B71184434CC Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
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
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран основной/используемый проект");

        SimplePaginationRequestModel req = new();
        IsBusyProgress = true;
        data = await FormsRepo.RequestQuestionnaires(req, ParentFormsPage.MainProject.Id);
        IsBusyProgress = false;

        if (data.Questionnaires is null)
        {
            SnackbarRepo.Add($"rest.Content.Questionnaires is null. error 62D3109B-7349-48E8-932B-762D5B0EA585", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return new TableData<ConstructorFormQuestionnaireModelDB>() { TotalItems = data.TotalRowsCount, Items = data.Questionnaires };
        }

        return new TableData<ConstructorFormQuestionnaireModelDB>() { TotalItems = data.TotalRowsCount, Items = data.Questionnaires };
    }

    /// <inheritdoc/>
    protected async Task QuestionnaireOpenDialog(ConstructorFormQuestionnaireModelDB? questionnaire = null)
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран основной/используемый проект");

        questionnaire ??= ConstructorFormQuestionnaireModelDB.BuildEmpty(ParentFormsPage.MainProject.Id);
        DialogParameters<EditQuestionnaireDialogComponent> parameters = new()
        {
            { x => x.Questionnaire, questionnaire },
            { x => x.ParentFormsPage, ParentFormsPage },
        };

        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult result = await DialogServiceRepo.Show<EditQuestionnaireDialogComponent>(questionnaire.Id < 1 ? "Создание новой анкеты/опроса" : $"Редактирование анкеты/опроса #{questionnaire.Id}", parameters, options).Result;
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
}