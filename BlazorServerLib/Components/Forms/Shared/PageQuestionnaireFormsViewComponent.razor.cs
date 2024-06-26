////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Page questionnaire forms - view
/// </summary>
public partial class PageQuestionnaireFormsViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    /// <summary>
    /// Questionnaire page
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabOfDocumentSchemeConstructorModelDB QuestionnairePage { get; set; }

    int _join_form_id;

    /// <summary>
    /// Join form
    /// </summary>
    protected void JoinFormHoldAction(int join_form_id)
    {
        _join_form_id = join_form_id;
        StateHasChanged();
    }

    /// <summary>
    /// Update page
    /// </summary>
    protected void UpdatePageAction(TabOfDocumentSchemeConstructorModelDB? page = null)
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
            FormQuestionnairePageResponseModel rest = await FormsRepo.GetQuestionnairePage(QuestionnairePage.Id);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            if (!rest.Success())
            {
                SnackbarRepo.Add($"Ошибка 16188CA3-EC20-4743-A31C-DA497CABDEB5 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            if (rest.QuestionnairePage is null)
            {
                SnackbarRepo.Add($"Ошибка E7427B3A-68CB-4560-B2E0-4AF69F2EDA72 [rest.Content.QuestionnairePage is null]", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            QuestionnairePage = rest.QuestionnairePage;
            QuestionnairePage.JoinsForms = QuestionnairePage.JoinsForms?.OrderBy(x => x.SortIndex).ToList();
            StateHasChanged();
        });
    }

    /// <summary>
    /// Форму можно сдвинуть выше?
    /// </summary>
    protected bool CanUpJoinForm(TabJoinDocumentSchemeConstructorModelDB pjf)
    {
        int min_index = QuestionnairePage.JoinsForms?.Any(x => x.Id != pjf.Id) == true
        ? QuestionnairePage.JoinsForms.Where(x => x.Id != pjf.Id).Min(x => x.SortIndex)
        : 1;
        return _join_form_id == 0 && pjf.SortIndex > min_index;
    }

    /// <summary>
    /// Форму можно сдвинуть ниже?
    /// </summary>
    protected bool CanDownJoinForm(TabJoinDocumentSchemeConstructorModelDB pjf)
    {
        int max_index = QuestionnairePage.JoinsForms?.Any(x => x.Id != pjf.Id) == true
        ? QuestionnairePage.JoinsForms.Where(x => x.Id != pjf.Id).Max(x => x.SortIndex)
        : 1;
        return _join_form_id == 0 && pjf.SortIndex < max_index;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (QuestionnairePage.JoinsForms is null)
        {
            SnackbarRepo.Add($"Дозагрузка `{nameof(QuestionnairePage.JoinsForms)}` в `{nameof(QuestionnairePage)} ['{QuestionnairePage.Name}' #{QuestionnairePage.Id}]`", Severity.Info, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = true;
            FormQuestionnairePageResponseModel rest = await FormsRepo.GetQuestionnairePage(QuestionnairePage.Id);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            QuestionnairePage.JoinsForms = rest.QuestionnairePage?.JoinsForms;
        }
    }
}