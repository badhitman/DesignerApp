using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class PageQuestionnaireViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<PageQuestionnaireViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;


    [CascadingParameter, EditorRequired]
    public ConstructorFormQuestionnairePageModelDB QuestionnairePage { get; set; } = default!;

    [Parameter, EditorRequired]
    public IEnumerable<ConstructorFormBaseModel> AllForms { get; set; } = default!;

    [Parameter, EditorRequired]
    public bool CanUpMove { get; set; }

    [Parameter, EditorRequired]
    public bool CanDownMove { get; set; }

    [Parameter, EditorRequired]
    public Action<int, ConstructorFormQuestionnairePageModelDB> SetIdForPageHandle { get; set; } = default!;

    [Parameter, EditorRequired]
    public Action<int, string?> SetNameForPageHandle { get; set; } = default!;

    [Parameter, EditorRequired]
    public Action QuestionnaireReloadHandle { get; set; } = default!;

    [Parameter, EditorRequired]
    public Action<bool> SetHoldHandle { get; set; } = default!;

    [Parameter, EditorRequired]
    public Action<ConstructorFormQuestionnaireModelDB, ConstructorFormQuestionnairePageModelDB?> UpdateQuestionnaireHandle { get; set; } = default!;

    protected int SelectedFormForAdding { get; set; }

    string? _name_orign { get; set; }
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
    public string? _description_orign { get; set; }
    public string? Description
    {
        get => _description_orign;
        set
        {
            QuestionnairePage.Description = value;
            SetHoldHandle(IsEdited);
        }
    }

    bool IsInitDelete = false;

    protected InputRichTextComponent? _currentTemplateInputRichText;

    protected bool CantSave => string.IsNullOrWhiteSpace(QuestionnairePage.Name) || (QuestionnairePage.Id > 0 && QuestionnairePage.Name == _name_orign && QuestionnairePage.Description == _description_orign && QuestionnairePage.Description == _description_orign);
    bool IsEdited => QuestionnairePage.Name != _name_orign || QuestionnairePage.Description != _description_orign;

    protected async Task MoveRow(VerticalDirectionsEnum direct)
    {
        IsBusyProgress = true;
        FormQuestionnaireResponseModel rest = await _forms.QuestionnairePageMove(QuestionnairePage.Id, direct);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{0C0C4DB1-0F94-48C6-88DE-9BF67A1DEA73}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.Questionnaire is null)
        {
            _snackbar.Add($"Ошибка {{66DE90D2-4BFD-4B1F-84B2-5E75B679B1F0}} Content [rest.Questionnaire is null]", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        UpdateQuestionnaireHandle(rest.Questionnaire, QuestionnairePage);
    }

    protected async Task Delete()
    {
        if (!IsInitDelete)
        {
            IsInitDelete = true;
            return;
        }
        IsBusyProgress = true;
        ResponseBaseModel rest = await _forms.DeleteQuestionnairePage(QuestionnairePage.Id);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{7B42BBCA-F6F0-4707-AD61-89A1171663BE}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        QuestionnaireReloadHandle();
    }

    protected void CancelEditing()
    {
        IsInitDelete = false;
        QuestionnairePage.Name = _name_orign ?? "";
        QuestionnairePage.Description = _description_orign;
        SetHoldHandle(IsEdited);
    }

    protected async Task AddFormToPage()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await _forms.CreateOrUpdateQuestionnairePageJoinForm(new ConstructorFormQuestionnairePageJoinFormModelDB()
        {
            FormId = SelectedFormForAdding,
            OwnerId = QuestionnairePage.Id,
            Name = ""
        });
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{16CFB88A-36B6-4D01-A7C7-059AAC0520EF}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        SelectedFormForAdding = 0;
        await ReloadPage();
    }

    protected async Task SavePage()
    {
        IsBusyProgress = true;
        FormQuestionnairePageResponseModel rest = await _forms.CreateOrUpdateQuestionnairePage(new EntryDescriptionOwnedModel() { Id = QuestionnairePage.Id, OwnerId = QuestionnairePage.OwnerId, Name = QuestionnairePage.Name, Description = QuestionnairePage.Description });
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{AF08ECED-2FDA-44BC-8E14-28ABC241833A}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.QuestionnairePage is null)
        {
            _snackbar.Add($"Ошибка {{5EB116FA-4949-41DC-B491-B1373147C87F}} rest.Content.QuestionnairePage is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        int i = QuestionnairePage.Id;
        QuestionnairePage.Id = rest.QuestionnairePage.Id;
        SetIdForPageHandle(i, rest.QuestionnairePage);

        _description_orign = Description;
        _name_orign = Name;
        IsInitDelete = false;

        SetHoldHandle(IsEdited);
    }

    async Task ReloadPage()
    {
        if (QuestionnairePage.Id < 1)
            return;

        IsBusyProgress = true;
        FormQuestionnairePageResponseModel rest = await _forms.GetQuestionnairePage(QuestionnairePage.Id);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{15A928CA-3C53-42D4-9442-3C5EBA7037DA}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.QuestionnairePage is null)
        {
            _snackbar.Add($"Ошибка {{B456FDD7-B0A4-45CA-88CB-F54759041B8B}} Content [rest.Content.QuestionnairePage is null]", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        QuestionnairePage.JoinsForms = rest.QuestionnairePage?.JoinsForms;
        QuestionnairePage.Owner = rest.QuestionnairePage?.Owner;
        SetIdForPageHandle(QuestionnairePage.Id, QuestionnairePage);
        StateHasChanged();
    }

    protected bool CheckRest(FormQuestionnairePageResponseModel rest)
    {

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{15A928CA-3C53-42D4-9442-3C5EBA7037DA}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return false;
        }
        if (rest.QuestionnairePage is null)
        {
            _snackbar.Add($"Ошибка {{B456FDD7-B0A4-45CA-88CB-F54759041B8B}} Content [rest.Content.QuestionnairePage is null]", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return false;
        }

        return true;
    }

    protected override async Task OnInitializedAsync()
    {
        await ReloadPage();
        _name_orign = QuestionnairePage.Name;
        _description_orign = QuestionnairePage.Description;
    }
}