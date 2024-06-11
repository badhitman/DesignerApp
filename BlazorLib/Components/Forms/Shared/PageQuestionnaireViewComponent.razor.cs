using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

/// <summary>
/// Page questionnaire view
/// </summary>
public partial class PageQuestionnaireViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    /// <summary>
    /// Questionnaire page
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormQuestionnairePageModelDB QuestionnairePage { get; set; }

    /// <summary>
    /// All forms
    /// </summary>
    [Parameter, EditorRequired]
    public required IEnumerable<ConstructorFormBaseModel> AllForms { get; set; }

    /// <summary>
    /// Can up move
    /// </summary>
    [Parameter, EditorRequired]
    public required bool CanUpMove { get; set; }

    /// <summary>
    /// Can down move
    /// </summary>
    [Parameter, EditorRequired]
    public bool CanDownMove { get; set; }

    /// <summary>
    /// Set id for page -  handle action
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<int, ConstructorFormQuestionnairePageModelDB> SetIdForPageHandle { get; set; }

    /// <summary>
    /// Set name for page - handle action
    /// </summary>
    [Parameter, EditorRequired]
    public Action<int, string?> SetNameForPageHandle { get; set; } = default!;

    /// <summary>
    /// Questionnaire reload - handle action
    /// </summary>
    [Parameter, EditorRequired]
    public Action QuestionnaireReloadHandle { get; set; } = default!;

    /// <summary>
    /// Set hold handle - action
    /// </summary>
    [Parameter, EditorRequired]
    public Action<bool> SetHoldHandle { get; set; } = default!;

    /// <summary>
    /// Update questionnaire - handle action
    /// </summary>
    [Parameter, EditorRequired]
    public Action<ConstructorFormQuestionnaireModelDB, ConstructorFormQuestionnairePageModelDB?> UpdateQuestionnaireHandle { get; set; } = default!;

    /// <summary>
    /// Selected form for adding
    /// </summary>
    protected int SelectedFormForAdding { get; set; }

    string? NameOrigin { get; set; }
    /// <summary>
    /// Name
    /// </summary>
    protected string? Name
    {
        get => QuestionnairePage.Name;
        set
        {
            QuestionnairePage.Name = value ?? "";
            SetNameForPageHandle(QuestionnairePage.Id, value);
            SetHoldHandle(IsEdited);
        }
    }
    string? DescriptionOrigin { get; set; }
    /// <summary>
    /// Описание
    /// </summary>
    public string? Description
    {
        get => DescriptionOrigin;
        set
        {
            QuestionnairePage.Description = value;
            SetHoldHandle(IsEdited);
        }
    }

    bool IsInitDelete = false;

    /// <summary>
    /// Current Template InputRichText ref
    /// </summary>
    protected InputRichTextComponent? _currentTemplateInputRichText_ref;

    /// <summary>
    /// Can`t save?
    /// </summary>
    protected bool CantSave => string.IsNullOrWhiteSpace(QuestionnairePage.Name) || (QuestionnairePage.Id > 0 && QuestionnairePage.Name == NameOrigin && QuestionnairePage.Description == DescriptionOrigin && QuestionnairePage.Description == DescriptionOrigin);
    bool IsEdited => QuestionnairePage.Name != NameOrigin || QuestionnairePage.Description != DescriptionOrigin;

    /// <summary>
    /// Перемещение страницы опроса/анкеты (сортировка страниц внутри опроса/анкеты)
    /// </summary>
    protected async Task MoveRow(VerticalDirectionsEnum direct)
    {
        IsBusyProgress = true;
        FormQuestionnaireResponseModel rest = await FormsRepo.QuestionnairePageMove(QuestionnairePage.Id, direct);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{0C0C4DB1-0F94-48C6-88DE-9BF67A1DEA73}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.Questionnaire is null)
        {
            SnackbarRepo.Add($"Ошибка {{66DE90D2-4BFD-4B1F-84B2-5E75B679B1F0}} Content [rest.Questionnaire is null]", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        UpdateQuestionnaireHandle(rest.Questionnaire, QuestionnairePage);
    }

    /// <summary>
    /// Delete
    /// </summary>
    protected async Task Delete()
    {
        if (!IsInitDelete)
        {
            IsInitDelete = true;
            return;
        }
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.DeleteQuestionnairePage(QuestionnairePage.Id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{7B42BBCA-F6F0-4707-AD61-89A1171663BE}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        QuestionnaireReloadHandle();
    }

    /// <summary>
    /// Отмена редактирования
    /// </summary>
    protected void CancelEditing()
    {
        IsInitDelete = false;
        QuestionnairePage.Name = NameOrigin ?? "";
        QuestionnairePage.Description = DescriptionOrigin;
        SetHoldHandle(IsEdited);
    }

    /// <summary>
    /// Add form to page
    /// </summary>
    protected async Task AddFormToPage()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.CreateOrUpdateQuestionnairePageJoinForm(new ConstructorFormQuestionnairePageJoinFormModelDB()
        {
            FormId = SelectedFormForAdding,
            OwnerId = QuestionnairePage.Id,
            Name = ""
        });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{16CFB88A-36B6-4D01-A7C7-059AAC0520EF}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        SelectedFormForAdding = 0;
        await ReloadPage();
    }

    /// <summary>
    /// Save page
    /// </summary>
    protected async Task SavePage()
    {
        IsBusyProgress = true;
        FormQuestionnairePageResponseModel rest = await FormsRepo.CreateOrUpdateQuestionnairePage(new EntryDescriptionOwnedModel() { Id = QuestionnairePage.Id, OwnerId = QuestionnairePage.OwnerId, Name = QuestionnairePage.Name, Description = QuestionnairePage.Description });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{AF08ECED-2FDA-44BC-8E14-28ABC241833A}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.QuestionnairePage is null)
        {
            SnackbarRepo.Add($"Ошибка {{5EB116FA-4949-41DC-B491-B1373147C87F}} rest.Content.QuestionnairePage is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        int i = QuestionnairePage.Id;
        QuestionnairePage.Id = rest.QuestionnairePage.Id;
        SetIdForPageHandle(i, rest.QuestionnairePage);

        DescriptionOrigin = Description;
        NameOrigin = Name;
        IsInitDelete = false;

        SetHoldHandle(IsEdited);
    }

    async Task ReloadPage()
    {
        if (QuestionnairePage.Id < 1)
            return;

        IsBusyProgress = true;
        FormQuestionnairePageResponseModel rest = await FormsRepo.GetQuestionnairePage(QuestionnairePage.Id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{15A928CA-3C53-42D4-9442-3C5EBA7037DA}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.QuestionnairePage is null)
        {
            SnackbarRepo.Add($"Ошибка {{B456FDD7-B0A4-45CA-88CB-F54759041B8B}} Content [rest.Content.QuestionnairePage is null]", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        QuestionnairePage.JoinsForms = rest.QuestionnairePage?.JoinsForms;
        QuestionnairePage.Owner = rest.QuestionnairePage?.Owner;
        SetIdForPageHandle(QuestionnairePage.Id, QuestionnairePage);
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReloadPage();
        NameOrigin = QuestionnairePage.Name;
        DescriptionOrigin = QuestionnairePage.Description;
    }
}