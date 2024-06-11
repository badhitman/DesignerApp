using BlazorLib;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Edit questionnaire dialog
/// </summary>
public partial class EditQuestionnaireDialogComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected IJSRuntime JsRuntimeRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required ConstructorFormQuestionnaireModelDB Questionnaire { get; set; }

    /// <inheritdoc/>
    protected bool IsEdited => Questionnaire.Name != QuestionnaireNameOrigin || Questionnaire.Description != QuestionnaireDescriptionOrigin;
    /// <inheritdoc/>
    protected PagesQuestionnairesViewComponent? pages_questionnaires_view_ref;
    /// <inheritdoc/>
    protected InputRichTextComponent? _currentTemplateInputRichText;

    string QuestionnaireNameOrigin { get; set; } = "";
    string? QuestionnaireDescriptionOrigin { get; set; }

    /// <inheritdoc/>
    protected void Close() => MudDialog.Close(DialogResult.Ok(Questionnaire));

    async Task ResetQuestionnaireForm()
    {
        if (Questionnaire.Id > 0)
        {
            IsBusyProgress = true;
            TResponseModel<ConstructorFormQuestionnaireModelDB> rest = await FormsRepo.GetQuestionnaire(Questionnaire.Id);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            if (rest.Response is null)
            {
                SnackbarRepo.Add($"rest.Content.Questionnaire is null. error {{2ECBE6F2-628E-4516-A0C4-B464BF1C915E}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            Questionnaire = rest.Response;
        }

        QuestionnaireNameOrigin = Questionnaire.Name;
        QuestionnaireDescriptionOrigin = Questionnaire.Description;
        pages_questionnaires_view_ref?.Update(Questionnaire);
        if (_currentTemplateInputRichText is not null)
            await JsRuntimeRepo.InvokeVoidAsync("CKEditorInterop.setValue", _currentTemplateInputRichText.UID, QuestionnaireDescriptionOrigin);
    }

    /// <inheritdoc/>
    protected async Task SaveQuestionnaire()
    {
        IsBusyProgress = true;
        TResponseModel<ConstructorFormQuestionnaireModelDB> rest = await FormsRepo.UpdateOrCreateQuestionnaire(new EntryDescriptionModel() { Id = Questionnaire.Id, Name = QuestionnaireNameOrigin, Description = QuestionnaireDescriptionOrigin });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{633F788F-4E1C-4008-8ADC-6DA4D8D836AA}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.Response is null)
        {
            SnackbarRepo.Add($"Ошибка {{B38A414C-4DBA-4F11-BB34-AA71F079F98D}} rest.Content.Questionnaire is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        Questionnaire = rest.Response;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ResetQuestionnaireForm();
        base.OnInitialized();
    }
}