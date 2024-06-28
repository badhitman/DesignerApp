////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.Document;

/// <summary>
/// Pages questionnaires view
/// </summary>
public partial class TabsOfDocumentsSchemesViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required DocumentSchemeConstructorModelDB DocumentScheme { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }


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
        return _tabs_is_hold && DocumentScheme.Pages?.FindIndex(x => x.Id == questionnaire_page_id) != QuestionnaireIndex;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран основной/используемый проект");

        IsBusyProgress = true;
        ConstructorFormsPaginationResponseModel rest = await FormsRepo.SelectForms(AltSimplePaginationRequestModel.Build(null, int.MaxValue, 0, true), ParentFormsPage.MainProject.Id);
        IsBusyProgress = false;

        if (rest.Elements is null)
            throw new Exception($"Ошибка 973D18EE-ED49-442D-B12B-CDC5A32C8A51 rest.Content.Elements is null");

        AllForms = rest.Elements;
    }

    /// <inheritdoc/>
    protected void QuestionnaireReloadAction()
    {
        IsBusyProgress = true;
        InvokeAsync(async () =>
        {
            TResponseModel<DocumentSchemeConstructorModelDB> rest = await FormsRepo.GetDocumentScheme(DocumentScheme.Id);
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            if (rest.Response is null)
            {
                SnackbarRepo.Add($"Ошибка A3D19BFC-38BB-4932-85DD-54C306D8C83E rest.Content.DocumentScheme", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            if (!rest.Success())
            {
                SnackbarRepo.Add($"Ошибка 55685D9E-E9D0-4937-A727-5BCC9FAD4381 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            DocumentScheme = rest.Response;
            StateHasChanged();
        });
    }

    /// <inheritdoc/>
    protected void SetIdForPageAction(int init_id, TabOfDocumentSchemeConstructorModelDB new_page)
    {
        if (PagesNotExist)
        {
            SnackbarRepo.Add($"PagesNotExist. Ошибка 6264F83F-DF3F-4860-BC18-5288AB335985", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        int i = DocumentScheme.Pages!.FindIndex(x => x.Id == init_id);
        if (i >= 0 && init_id != new_page.Id)
            new_page.Id = i;

        i = DocumentScheme.Pages.FindIndex(x => x.Id == new_page.Id);
        if (i >= 0)
            DocumentScheme.Pages[i].JoinsForms = new_page.JoinsForms;

        StateHasChanged();
    }

    /// <inheritdoc/>
    protected bool CanUpPage(TabOfDocumentSchemeConstructorModelDB questionnaire_page) => questionnaire_page.SortIndex > (DocumentScheme.Pages!.Any(x => x.Id != questionnaire_page.Id) ? DocumentScheme.Pages!.Where(x => x.Id != questionnaire_page.Id)!.Min(y => y.SortIndex) : 1) && !DocumentScheme.Pages!.Any(x => x.Id < 1);
    /// <inheritdoc/>
    protected bool CanDownPage(TabOfDocumentSchemeConstructorModelDB questionnaire_page) => questionnaire_page.SortIndex < (DocumentScheme.Pages!.Any(x => x.Id != questionnaire_page.Id) ? DocumentScheme.Pages!.Where(x => x.Id != questionnaire_page.Id)!.Max(y => y.SortIndex) : DocumentScheme.Pages!.Count) && !DocumentScheme.Pages!.Any(x => x.Id < 1);

    bool PagesNotExist => DocumentScheme.Pages is null || DocumentScheme.Pages.Count == 0;

    /// <inheritdoc/>
    protected void SetNameForPage(int id, string name)
    {
        if (PagesNotExist)
        {
            SnackbarRepo.Add($"PagesNotExist. Ошибка 5FD8058E-50B0-49DA-8808-DB7C2BED80C2", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        TabOfDocumentSchemeConstructorModelDB? _page = DocumentScheme.Pages!.FirstOrDefault(x => x.Id == id);
        if (_page is null)
        {
            SnackbarRepo.Add($"_page is null. Ошибка CBDF9789-4A28-4869-A214-A4702A432DA6", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        _page.Name = name;
        StateHasChanged();
    }

    /// <inheritdoc/>
    public void AddTab()
    {
        DocumentScheme.Pages ??= [];
        int new_page_id = PagesNotExist
            ? 0
            : DocumentScheme.Pages.Min(x => x.Id) - 1;

        if (new_page_id > 0)
            new_page_id = 0;

        int i = 1;
        while (i <= 100 && DocumentScheme.Pages.Any(x => x.Name.Equals($"New {i}", StringComparison.OrdinalIgnoreCase)))
            i++;

        DocumentScheme.Pages.Add(new TabOfDocumentSchemeConstructorModelDB { OwnerId = DocumentScheme.Id, Id = new_page_id, Name = $"New {(i < 100 ? i.ToString() : Guid.NewGuid().ToString())}", JoinsForms = new(), SortIndex = (DocumentScheme.Pages.Any() ? DocumentScheme.Pages.Max(x => x.SortIndex) + 1 : 1) });
        QuestionnaireIndex = DocumentScheme.Pages.Count - 1;
        _stateHasChanged = true;
    }

    /// <inheritdoc/>
    public void Update(DocumentSchemeConstructorModelDB document_scheme, TabOfDocumentSchemeConstructorModelDB? page = null)
    {
        DocumentScheme = document_scheme;
        if (page is not null && DocumentScheme.Pages?.Any(x => x.Id == page.Id) == true)
            QuestionnaireIndex = DocumentScheme.Pages.FindIndex(x => x.Id == page.Id);
        StateHasChanged();
    }

    /// <inheritdoc/>
    public void RemoveTab(string id)
    {
        TabOfDocumentSchemeConstructorModelDB? tabView = DocumentScheme.Pages!.SingleOrDefault((t) => Equals(t.Id, id));
        if (tabView is not null)
        {
            DocumentScheme.Pages!.Remove(tabView);
            _stateHasChanged = true;
        }
    }
    /// <inheritdoc/>
    protected void CloseTabCallback(MudTabPanel panel) => RemoveTab(panel.ID.ToString() ?? throw new Exception("tab engine error"));
}