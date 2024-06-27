////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Page questionnaire form main view
/// </summary>
public partial class PageQuestionnaireFormMainViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ILogger<PageQuestionnaireFormMainViewComponent> LoggerRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public required TabOfDocumentSchemeConstructorModelDB QuestionnairePage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required TabJoinDocumentSchemeConstructorModelDB PageJoinForm { get; set; }

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
    public required Action<TabOfDocumentSchemeConstructorModelDB?> UpdatePageActionHandle { get; set; }

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
            SnackbarRepo.Add($"Ошибка BC47C379-C7EA-4527-BC66-E7E18E13D55E Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
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
            SnackbarRepo.Add($"Ошибка 82D66555-EE5A-44CD-94C3-C7D30C06348A Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        UpdatePageActionHandle(null);
    }

    /// <inheritdoc/>
    protected async Task SaveJoinForm()
    {
        IsBusyProgress = true;
        TabJoinDocumentSchemeConstructorModelDB req = new()
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
            SnackbarRepo.Add($"Ошибка B384A2CC-A7BE-4335-976C-A2612FE84723 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
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
            TResponseModel<FormConstructorModelDB> rest = await FormsRepo.GetForm(PageJoinForm.FormId);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            PageJoinForm.Form = rest.Response;
        }
    }
}