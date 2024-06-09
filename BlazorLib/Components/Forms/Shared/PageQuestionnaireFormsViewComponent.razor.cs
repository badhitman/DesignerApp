using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class PageQuestionnaireFormsViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<PageQuestionnaireFormsViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;


    [CascadingParameter, EditorRequired]
    public ConstructorFormQuestionnairePageModelDB QuestionnairePage { get; set; } = default!;

    protected bool SetTitleForm { get; set; }

    int _join_form_id;
    protected void JoinFormHoldAction(int join_form_id)
    {
        _join_form_id = join_form_id;
        StateHasChanged();
    }

    protected void UpdatePageAction(ConstructorFormQuestionnairePageModelDB? page = null)
    {
        if (page is not null)
        {
            QuestionnairePage = page;
            StateHasChanged();
            return;
        }
        IsBusyProgress = true;
        _ = InvokeAsync(async () =>
        {
            FormQuestionnairePageResponseModel rest = await _forms.GetQuestionnairePage(QuestionnairePage.Id);
            IsBusyProgress = false;

            _snackbar.ShowMessagesResponse(rest.Messages);
            if (!rest.Success())
            {
                _snackbar.Add($"Ошибка {{566396FB-843B-4C07-AE89-D98D7DD268CD}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            if (rest.QuestionnairePage is null)
            {
                _snackbar.Add($"Ошибка {{C58098C7-FEAA-4BD5-9E30-48FA91DBBF65}} [rest.Content.QuestionnairePage is null]", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            QuestionnairePage = rest.QuestionnairePage;
            QuestionnairePage.JoinsForms = QuestionnairePage.JoinsForms?.OrderBy(x => x.SortIndex).ToList();
            StateHasChanged();
        });
    }

    protected bool CanUpJoinForm(ConstructorFormQuestionnairePageJoinFormModelDB pjf)
    {
        int min_index = QuestionnairePage.JoinsForms?.Any(x => x.Id != pjf.Id) == true
        ? QuestionnairePage.JoinsForms.Where(x => x.Id != pjf.Id).Min(x => x.SortIndex)
        : 1;
        return _join_form_id == 0 && pjf.SortIndex > min_index;
    }

    protected bool CanDownJoinForm(ConstructorFormQuestionnairePageJoinFormModelDB pjf)
    {
        int max_index = QuestionnairePage.JoinsForms?.Any(x => x.Id != pjf.Id) == true
        ? QuestionnairePage.JoinsForms.Where(x => x.Id != pjf.Id).Max(x => x.SortIndex)
        : 1;
        return _join_form_id == 0 && pjf.SortIndex < max_index;
    }

    protected override async Task OnInitializedAsync()
    {
        if (QuestionnairePage.JoinsForms is null)
        {
            _snackbar.Add($"Дозагрузка `{nameof(QuestionnairePage.JoinsForms)}` в `{nameof(QuestionnairePage)} ['{QuestionnairePage.Name}' #{QuestionnairePage.Id}]`", Severity.Info, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = true;
            FormQuestionnairePageResponseModel rest = await _forms.GetQuestionnairePage(QuestionnairePage.Id);
            IsBusyProgress = false;

            _snackbar.ShowMessagesResponse(rest.Messages);
            QuestionnairePage.JoinsForms = rest.QuestionnairePage?.JoinsForms;
        }
    }
}