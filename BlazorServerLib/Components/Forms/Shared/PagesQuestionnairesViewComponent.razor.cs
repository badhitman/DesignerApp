using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Pages questionnaires view
/// </summary>
public partial class PagesQuestionnairesViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required ConstructorFormQuestionnaireModelDB Questionnaire { get; set; }

    /// <inheritdoc/>
    public MudDynamicTabs? DynamicTabs;
    /// <inheritdoc/>
    protected bool _stateHasChanged;
    /// <inheritdoc/>
    public int QuestionnaireIndex;

    /// <inheritdoc/>
    protected IEnumerable<ConstructorFormBaseModel> AllForms = default!;

    bool _tabs_is_hold = false;
    /// <inheritdoc/>
    protected void SetHoldAction(bool _is_hold)
    {
        _tabs_is_hold = _is_hold;
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected bool TabIsDisabled(int questionnaire_page_id)
    {
        return _tabs_is_hold && Questionnaire.Pages?.FindIndex(x => x.Id == questionnaire_page_id) != QuestionnaireIndex;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        ConstructorFormsPaginationResponseModel rest = await FormsRepo.SelectForms(new() { PageSize = 1000, StrongMode = true });
        IsBusyProgress = false;

        if (rest.Elements is null)
            throw new Exception($"Ошибка {{3E9B0E59-6DDA-47A3-B77B-25316A29EE37}} rest.Content.Elements is null");

        AllForms = rest.Elements;
    }

    /// <inheritdoc/>
    protected void QuestionnaireReloadAction()
    {
        IsBusyProgress = true;
        InvokeAsync(async () =>
        {
            FormQuestionnaireResponseModel rest = await FormsRepo.GetQuestionnaire(Questionnaire.Id);
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            if (rest.Questionnaire is null)
            {
                SnackbarRepo.Add($"Ошибка {{1D342ED5-B3DC-4760-A684-33D875EF6AB4}} rest.Content.Questionnaire", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            if (!rest.Success())
            {
                SnackbarRepo.Add($"Ошибка {{2D3A1007-9B65-4428-BE4C-803934433B66}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            Questionnaire = rest.Questionnaire;
            StateHasChanged();
        });
    }

    /// <inheritdoc/>
    protected void SetIdForPageAction(int init_id, ConstructorFormQuestionnairePageModelDB new_page)
    {
        if (PagesNotExist)
        {
            SnackbarRepo.Add($"PagesNotExist. Ошибка {{2AE5D2B1-73FF-4A57-A82D-594B278D2563}}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        int i = Questionnaire.Pages!.FindIndex(x => x.Id == init_id);
        if (i >= 0 && init_id != new_page.Id)
            new_page.Id = i;

        i = Questionnaire.Pages.FindIndex(x => x.Id == new_page.Id);
        if (i >= 0)
            Questionnaire.Pages[i].JoinsForms = new_page.JoinsForms;

        StateHasChanged();
    }

    /// <inheritdoc/>
    protected bool CanUpPage(ConstructorFormQuestionnairePageModelDB questionnaire_page) => questionnaire_page.SortIndex > (Questionnaire.Pages!.Any(x => x.Id != questionnaire_page.Id) ? Questionnaire.Pages!.Where(x => x.Id != questionnaire_page.Id)!.Min(y => y.SortIndex) : 1) && !Questionnaire.Pages!.Any(x => x.Id < 1);
    /// <inheritdoc/>
    protected bool CanDownPage(ConstructorFormQuestionnairePageModelDB questionnaire_page) => questionnaire_page.SortIndex < (Questionnaire.Pages!.Any(x => x.Id != questionnaire_page.Id) ? Questionnaire.Pages!.Where(x => x.Id != questionnaire_page.Id)!.Max(y => y.SortIndex) : Questionnaire.Pages!.Count) && !Questionnaire.Pages!.Any(x => x.Id < 1);

    bool PagesNotExist => Questionnaire.Pages is null || Questionnaire.Pages.Count == 0;

    /// <inheritdoc/>
    protected void SetNameForPage(int id, string name)
    {
        if (PagesNotExist)
        {
            SnackbarRepo.Add($"PagesNotExist. Ошибка {{894D5FAA-5390-42BB-A0FB-3E9B1FBED810}}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        ConstructorFormQuestionnairePageModelDB? _page = Questionnaire.Pages!.FirstOrDefault(x => x.Id == id);
        if (_page is null)
        {
            SnackbarRepo.Add($"_page is null. Ошибка {{7165030D-4A99-4E8F-8315-BC6B1673239E}}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        _page.Name = name;
        StateHasChanged();
    }

    /// <inheritdoc/>
    public void AddTab()
    {
        Questionnaire.Pages ??= [];
        int new_page_id = PagesNotExist 
            ? 0 
            : Questionnaire.Pages.Min(x => x.Id) - 1;

        if (new_page_id > 0)
            new_page_id = 0;

        int i = 1;
        while (i <= 100 && Questionnaire.Pages.Any(x => x.Name.Equals($"New {i}", StringComparison.OrdinalIgnoreCase)))
            i++;

        Questionnaire.Pages.Add(new ConstructorFormQuestionnairePageModelDB { OwnerId = Questionnaire.Id, Id = new_page_id, Name = $"New {(i < 100 ? i.ToString() : Guid.NewGuid().ToString())}", JoinsForms = new(), SortIndex = (Questionnaire.Pages.Any() ? Questionnaire.Pages.Max(x => x.SortIndex) + 1 : 1) });
        QuestionnaireIndex = Questionnaire.Pages.Count - 1;
        _stateHasChanged = true;
    }

    /// <inheritdoc/>
    public void Update(ConstructorFormQuestionnaireModelDB questionnaire, ConstructorFormQuestionnairePageModelDB? page = null)
    {
        Questionnaire = questionnaire;
        if (page is not null && Questionnaire.Pages?.Any(x => x.Id == page.Id) == true)
            QuestionnaireIndex = Questionnaire.Pages.FindIndex(x => x.Id == page.Id);
        StateHasChanged();
    }

    /// <inheritdoc/>
    public void RemoveTab(int id)
    {
        ConstructorFormQuestionnairePageModelDB? tabView = Questionnaire.Pages!.SingleOrDefault((t) => Equals(t.Id, id));
        if (tabView is not null)
        {
            Questionnaire.Pages!.Remove(tabView);
            _stateHasChanged = true;
        }
    }
    /// <inheritdoc/>
    protected void CloseTabCallback(MudTabPanel panel) => RemoveTab((int)panel.ID);
}