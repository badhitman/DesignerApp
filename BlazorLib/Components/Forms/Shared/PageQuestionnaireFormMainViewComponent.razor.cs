using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class PageQuestionnaireFormMainViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<PageQuestionnaireFormMainViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;


    [CascadingParameter, EditorRequired]
    public ConstructorFormQuestionnairePageModelDB QuestionnairePage { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public ConstructorFormQuestionnairePageJoinFormModelDB PageJoinForm { get; set; } = default!;

    [Parameter, EditorRequired]
    public bool CanUp { get; set; }

    [Parameter, EditorRequired]
    public bool CanDown { get; set; }

    [Parameter, EditorRequired]
    public int CurrentFormJoinEdit { get; set; }

    [Parameter, EditorRequired]
    public Action<int> JoinFormHoldHandle { get; set; } = default!;

    [Parameter, EditorRequired]
    public Action<ConstructorFormQuestionnairePageModelDB?> UpdatePageActionHandle { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public bool InUse { get; set; } = default!;

    protected async Task DeleteJoinForm()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await _forms.DeleteQuestionnairePageJoinForm(PageJoinForm.Id);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{547E6D9E-0E8C-415F-BC53-9A925A8C4E90}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        UpdatePageActionHandle(null);
    }

    protected string TitleFormJoin => PageJoinForm.ShowTitle == true ? string.IsNullOrWhiteSpace(PageJoinForm.Name) ? PageJoinForm.Form!.Name : PageJoinForm.Name : "";

    bool _join_set_title_orign = false;
    bool SetTitleForm
    {
        get => PageJoinForm.ShowTitle == true;
        set
        {
            PageJoinForm.ShowTitle = value;
            JoinFormHoldHandle(IsEdited ? PageJoinForm.Id : 0);
        }
    }

    string _join_name_orign = "";
    string PageJoinFormName
    {
        get => PageJoinForm.Name;
        set
        {
            PageJoinForm.Name = value;
            JoinFormHoldHandle(IsEdited ? PageJoinForm.Id : 0);
        }
    }

    bool _is_table_orign;
    bool IsTable
    {
        get => PageJoinForm.IsTable;
        set
        {
            PageJoinForm.IsTable = value;
            JoinFormHoldHandle(IsEdited ? PageJoinForm.Id : 0);
        }
    }

    protected bool IsDisabled => CurrentFormJoinEdit > 0 && CurrentFormJoinEdit != PageJoinForm.Id;
    bool IsEdited => _join_name_orign != PageJoinFormName || SetTitleForm != _join_set_title_orign || IsTable != _is_table_orign;

    protected async Task QuestionnairePageJoinFormMove(VerticalDirectionsEnum direct)
    {
        IsBusyProgress = true;
        FormQuestionnairePageResponseModel rest = await _forms.QuestionnairePageJoinFormMove(PageJoinForm.Id, direct);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{EC9DD6E2-5359-48EE-AEE5-04EF64BF8139}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        UpdatePageActionHandle(null);
    }

    protected async Task SaveJoinForm()
    {
        IsBusyProgress = true;
        ConstructorFormQuestionnairePageJoinFormModelDB req = new()
        {
            Description = PageJoinForm.Description,
            FormId = PageJoinForm.FormId,
            Id = PageJoinForm.Id,
            IsTable = PageJoinForm.IsTable,
            Name = PageJoinForm.Name,
            OwnerId = PageJoinForm.OwnerId,
            ShowTitle = PageJoinForm.ShowTitle,
            SortIndex = PageJoinForm.SortIndex
        };
        ResponseBaseModel rest = await _forms.CreateOrUpdateQuestionnairePageJoinForm(req);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{1380C317-88CC-42AC-9188-896C7186B133}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        _join_name_orign = PageJoinForm.Name;
        _join_set_title_orign = PageJoinForm.ShowTitle == true;
        _is_table_orign = PageJoinForm.IsTable;
        JoinFormHoldHandle(IsEdited ? PageJoinForm.Id : 0);
        UpdatePageActionHandle(null);
    }

    protected void ResetFormJoin()
    {
        PageJoinForm.Name = _join_name_orign;
        PageJoinForm.ShowTitle = _join_set_title_orign;
        PageJoinForm.IsTable = _is_table_orign;
        JoinFormHoldHandle(0);
    }

    protected override async Task OnInitializedAsync()
    {
        _join_name_orign = PageJoinForm.Name;
        _join_set_title_orign = PageJoinForm.ShowTitle == true;
        _is_table_orign = PageJoinForm.IsTable;

        if (PageJoinForm.Form is null)
        {
            _logger.LogWarning("Дозагрузка [Form] для [PageJoinForm]...");
            IsBusyProgress = true;
            FormResponseModel rest = await _forms.GetForm(PageJoinForm.FormId);
            IsBusyProgress = false;

            _snackbar.ShowMessagesResponse(rest.Messages);
            PageJoinForm.Form = rest.Form;
        }
    }
}