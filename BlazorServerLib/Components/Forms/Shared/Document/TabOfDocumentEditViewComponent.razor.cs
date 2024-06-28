////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.Document;

/// <summary>
/// Page questionnaire view
/// </summary>
public partial class TabOfDocumentEditViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;


    /// <summary>
    /// DocumentScheme page
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabOfDocumentSchemeConstructorModelDB QuestionnairePage { get; set; }

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
    public required Action<int, TabOfDocumentSchemeConstructorModelDB> SetIdForPageHandle { get; set; }

    /// <summary>
    /// Set name for page - handle action
    /// </summary>
    [Parameter, EditorRequired]
    public Action<int, string?> SetNameForPageHandle { get; set; } = default!;

    /// <summary>
    /// DocumentScheme reload - handle action
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
    public Action<DocumentSchemeConstructorModelDB, TabOfDocumentSchemeConstructorModelDB?> UpdateQuestionnaireHandle { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public UserInfoModel? CurrentUser { get; set; }


    int _selectedFormForAdding;
    /// <summary>
    /// Selected form for adding
    /// </summary>
    protected int SelectedFormForAdding
    {
        get => _selectedFormForAdding;
        set
        {
            _selectedFormForAdding = value;

            if (_selectedFormForAdding < 1)
                addingFormToTabPageName = null;
        }
    }

    // 

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
        if (CurrentUser is null)
            throw new Exception("CurrentUser is null");

        IsBusyProgress = true;
        TResponseModel<DocumentSchemeConstructorModelDB> rest = await FormsRepo.MoveTabOfDocumentScheme(QuestionnairePage.Id, direct);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
            return;
        }
        if (rest.Response is null)
        {
            SnackbarRepo.Add($"Ошибка 671CB343-ADD5-46AE-91F8-24175FBF2592 Content [rest.DocumentScheme is null]", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        UpdateQuestionnaireHandle(rest.Response, QuestionnairePage);
    }

    /// <summary>
    /// Delete
    /// </summary>
    protected async Task Delete()
    {
        if (CurrentUser is null)
            throw new Exception("CurrentUser is null");

        if (!IsInitDelete)
        {
            IsInitDelete = true;
            return;
        }
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.DeleteTabOfDocumentScheme(QuestionnairePage.Id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
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

    string? addingFormToTabPageName;

    /// <summary>
    /// Add form to page
    /// </summary>
    protected async Task AddFormToPage()
    {
        if (CurrentUser is null)
            throw new Exception("CurrentUser is null");

        if (string.IsNullOrWhiteSpace(addingFormToTabPageName))
        {
            SnackbarRepo.Error("Укажите название");
            return;
        }

        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.CreateOrUpdateTabDocumentSchemeJoinForm(new TabJoinDocumentSchemeConstructorModelDB()
        {
            FormId = SelectedFormForAdding,
            OwnerId = QuestionnairePage.Id,
            Name = addingFormToTabPageName,
        });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
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
        if (CurrentUser is null)
            throw new Exception("CurrentUser is null");

        IsBusyProgress = true;
        TabOfDocumentSchemeResponseModel rest = await FormsRepo.CreateOrUpdateTabOfDocumentScheme(new EntryDescriptionOwnedModel() { Id = QuestionnairePage.Id, OwnerId = QuestionnairePage.OwnerId, Name = QuestionnairePage.Name, Description = QuestionnairePage.Description });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
            return;
        }
        if (rest.TabOfDocumentScheme is null)
        {
            SnackbarRepo.Add($"Ошибка 07653445-0B30-46CB-9B79-3B068BAB9AEB rest.Content.QuestionnairePage is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        int i = QuestionnairePage.Id;
        QuestionnairePage.Id = rest.TabOfDocumentScheme.Id;
        SetIdForPageHandle(i, rest.TabOfDocumentScheme);

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
        TabOfDocumentSchemeResponseModel rest = await FormsRepo.GetTabOfDocumentScheme(QuestionnairePage.Id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка 815BCE17-9180-4C27-8016-BEB5244A3454 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.TabOfDocumentScheme is null)
        {
            SnackbarRepo.Add($"Ошибка 5B879025-EC6E-4989-9A75-5844BD20DF0B Content [rest.Content.QuestionnairePage is null]", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        QuestionnairePage.JoinsForms = rest.TabOfDocumentScheme?.JoinsForms;
        QuestionnairePage.Owner = rest.TabOfDocumentScheme?.Owner;
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