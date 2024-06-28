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
/// Edit questionnaire dialog
/// </summary>
public partial class EditDocumentSchemeDialogComponent : BlazorBusyComponentBaseModel
{
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
    public required DocumentSchemeConstructorModelDB DocumentScheme { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public UserInfoModel? CurrentUser { get; set; }


    /// <inheritdoc/>
    protected bool IsEdited => DocumentScheme.SystemName != QuestionnaireSystemNameOrigin || DocumentScheme.Name != QuestionnaireNameOrigin || DocumentScheme.Description != QuestionnaireDescriptionOrigin;
    /// <inheritdoc/>
    protected TabsOfDocumentsSchemesViewComponent? pages_questionnaires_view_ref;
    /// <inheritdoc/>
    protected InputRichTextComponent? _currentTemplateInputRichText;


    string QuestionnaireSystemNameOrigin { get; set; } = "";
    string QuestionnaireNameOrigin { get; set; } = "";
    string? QuestionnaireDescriptionOrigin { get; set; }

    /// <inheritdoc/>
    protected void Close() => MudDialog.Close(DialogResult.Ok(DocumentScheme));

    async Task ResetQuestionnaireForm()
    {
        if (DocumentScheme.Id > 0)
        {
            IsBusyProgress = true;
            TResponseModel<DocumentSchemeConstructorModelDB> rest = await FormsRepo.GetDocumentScheme(DocumentScheme.Id);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            if (rest.Response is null)
            {
                SnackbarRepo.Add($"rest.Content.DocumentScheme is null. error 84DC51AA-74C1-4FA1-B9C6-B60548C10820", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            DocumentScheme = rest.Response;
        }

        QuestionnaireSystemNameOrigin = DocumentScheme.SystemName;
        QuestionnaireNameOrigin = DocumentScheme.Name;
        QuestionnaireDescriptionOrigin = DocumentScheme.Description;

        pages_questionnaires_view_ref?.Update(DocumentScheme);
        _currentTemplateInputRichText?.SetValue(QuestionnaireDescriptionOrigin);
    }

    /// <inheritdoc/>
    protected async Task SaveQuestionnaire()
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран основной/используемый проект");

        IsBusyProgress = true;
        TResponseModel<DocumentSchemeConstructorModelDB> rest = await FormsRepo.UpdateOrCreateDocumentScheme(new EntryConstructedModel() { Id = DocumentScheme.Id, SystemName = QuestionnaireSystemNameOrigin, Name = QuestionnaireNameOrigin, Description = QuestionnaireDescriptionOrigin, ProjectId = ParentFormsPage.MainProject.Id });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка C7172378-05A5-4547-ADA4-EA15B84C2CE1 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.Response is null)
        {
            SnackbarRepo.Add($"Ошибка FCB62EA3-689E-4222-9D59-8D1DEF18CFC5 rest.Content.DocumentScheme is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        DocumentScheme = rest.Response;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ResetQuestionnaireForm();
        base.OnInitialized();
    }
}