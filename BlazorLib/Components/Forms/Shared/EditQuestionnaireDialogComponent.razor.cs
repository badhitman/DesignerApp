using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class EditQuestionnaireDialogComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<EditQuestionnaireDialogComponent> _logger { get; set; } = default!;

    [Inject]
    protected IJSRuntime _js_runtime { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter, EditorRequired]
    public ConstructorFormQuestionnaireModelDB Questionnaire { get; set; } = default!;

    protected bool IsEdited => Questionnaire.Name != questionnaire_name_orign || Questionnaire.Description != questionnaire_description_orign;
    protected PagesQuestionnairesViewComponent? pages_questionnaires_view_ref;
    protected InputRichTextComponent? _currentTemplateInputRichText;

    string questionnaire_name_orign { get; set; } = "";
    string? questionnaire_description_orign { get; set; }

    protected void Close() => MudDialog.Close(DialogResult.Ok(Questionnaire));

    async Task ResetQuestionnaireForm()
    {
        if (Questionnaire.Id > 0)
        {
            IsBusyProgress = true;
            FormQuestionnaireResponseModel rest = await _forms.GetQuestionnaire(Questionnaire.Id);
            IsBusyProgress = false;

            _snackbar.ShowMessagesResponse(rest.Messages);
            if (rest.Questionnaire is null)
            {
                _snackbar.Add($"rest.Content.Questionnaire is null. error {{2ECBE6F2-628E-4516-A0C4-B464BF1C915E}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            Questionnaire = rest.Questionnaire;
        }

        questionnaire_name_orign = Questionnaire.Name;
        questionnaire_description_orign = Questionnaire.Description;
        pages_questionnaires_view_ref?.Update(Questionnaire);
        if (_currentTemplateInputRichText is not null)
            await _js_runtime.InvokeVoidAsync("CKEditorInterop.setValue", _currentTemplateInputRichText.UID, questionnaire_description_orign);
    }

    protected async Task SaveQuestionnaire()
    {
        IsBusyProgress = true;
        FormQuestionnaireResponseModel rest = await _forms.UpdateOrCreateQuestionnaire(new EntryDescriptionModel() { Id = Questionnaire.Id, Name = questionnaire_name_orign, Description = questionnaire_description_orign });
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{633F788F-4E1C-4008-8ADC-6DA4D8D836AA}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.Questionnaire is null)
        {
            _snackbar.Add($"Ошибка {{B38A414C-4DBA-4F11-BB34-AA71F079F98D}} rest.Content.Questionnaire is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        Questionnaire = rest.Questionnaire;
    }

/// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ResetQuestionnaireForm();
        base.OnInitialized();
    }
}