using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

/// <summary>
/// Page questionnaire form main view
/// </summary>
public partial class PageQuestionnaireFormMainViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ILogger<PageQuestionnaireFormMainViewComponent> LoggerRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter]
    public required ConstructorFormQuestionnairePageModelDB QuestionnairePage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormQuestionnairePageJoinFormModelDB PageJoinForm { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public bool CanUp { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public bool CanDown { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public int CurrentFormJoinEdit { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required Action<int> JoinFormHoldHandle { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required Action<ConstructorFormQuestionnairePageModelDB?> UpdatePageActionHandle { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public bool InUse { get; set; } = default!;

    /// <inheritdoc/>
    protected async Task DeleteJoinForm()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.DeleteQuestionnairePageJoinForm(PageJoinForm.Id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{547E6D9E-0E8C-415F-BC53-9A925A8C4E90}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        UpdatePageActionHandle(null);
    }

    /// <inheritdoc/>
    protected string TitleFormJoin => PageJoinForm.ShowTitle == true ? string.IsNullOrWhiteSpace(PageJoinForm.Name) ? PageJoinForm.Form!.Name : PageJoinForm.Name : "";

    bool _join_set_title_origin = false;
    bool SetTitleForm
    {
        get => PageJoinForm.ShowTitle == true;
        set
        {
            PageJoinForm.ShowTitle = value;
            JoinFormHoldHandle(IsEdited ? PageJoinForm.Id : 0);
        }
    }

    string _join_name_origin = "";
    string PageJoinFormName
    {
        get => PageJoinForm.Name;
        set
        {
            PageJoinForm.Name = value;
            JoinFormHoldHandle(IsEdited ? PageJoinForm.Id : 0);
        }
    }

    bool _is_table_origin;
    bool IsTable
    {
        get => PageJoinForm.IsTable;
        set
        {
            PageJoinForm.IsTable = value;
            JoinFormHoldHandle(IsEdited ? PageJoinForm.Id : 0);
        }
    }

    /// <inheritdoc/>
    protected bool IsDisabled => CurrentFormJoinEdit > 0 && CurrentFormJoinEdit != PageJoinForm.Id;
    bool IsEdited => _join_name_origin != PageJoinFormName || SetTitleForm != _join_set_title_origin || IsTable != _is_table_origin;

    /// <inheritdoc/>
    protected async Task QuestionnairePageJoinFormMove(VerticalDirectionsEnum direct)
    {
        IsBusyProgress = true;
        FormQuestionnairePageResponseModel rest = await FormsRepo.QuestionnairePageJoinFormMove(PageJoinForm.Id, direct);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{EC9DD6E2-5359-48EE-AEE5-04EF64BF8139}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        UpdatePageActionHandle(null);
    }

    /// <inheritdoc/>
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
        ResponseBaseModel rest = await FormsRepo.CreateOrUpdateQuestionnairePageJoinForm(req);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{1380C317-88CC-42AC-9188-896C7186B133}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        _join_name_origin = PageJoinForm.Name;
        _join_set_title_origin = PageJoinForm.ShowTitle == true;
        _is_table_origin = PageJoinForm.IsTable;
        JoinFormHoldHandle(IsEdited ? PageJoinForm.Id : 0);
        UpdatePageActionHandle(null);
    }

    /// <inheritdoc/>
    protected void ResetFormJoin()
    {
        PageJoinForm.Name = _join_name_origin;
        PageJoinForm.ShowTitle = _join_set_title_origin;
        PageJoinForm.IsTable = _is_table_origin;
        JoinFormHoldHandle(0);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        _join_name_origin = PageJoinForm.Name;
        _join_set_title_origin = PageJoinForm.ShowTitle == true;
        _is_table_origin = PageJoinForm.IsTable;

        if (PageJoinForm.Form is null)
        {
            LoggerRepo.LogWarning("Дозагрузка [Form] для [PageJoinForm]...");
            IsBusyProgress = true;
            FormResponseModel rest = await FormsRepo.GetForm(PageJoinForm.FormId);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            PageJoinForm.Form = rest.Form;
        }
    }
}