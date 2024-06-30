////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.Document;

/// <summary>
/// Page questionnaire view
/// </summary>
public partial class TabOfDocumentEditViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IConstructorService ConstructorRepo { get; set; } = default!;


    /// <summary>
    /// DocumentScheme page
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabOfDocumentSchemeConstructorModelDB DocumentPage { get; set; }

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
    public Action DocumentReloadHandle { get; set; } = default!;

    /// <summary>
    /// Set hold handle - action
    /// </summary>
    [Parameter, EditorRequired]
    public Action<bool> SetHoldHandle { get; set; } = default!;

    /// <summary>
    /// Update questionnaire - handle action
    /// </summary>
    [Parameter, EditorRequired]
    public Action<DocumentSchemeConstructorModelDB, TabOfDocumentSchemeConstructorModelDB?> UpdateDocumentHandle { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }

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
        get => DocumentPage.Name;
        set
        {
            DocumentPage.Name = value ?? "";
            SetNameForPageHandle(DocumentPage.Id, value);
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
            DocumentPage.Description = value;
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
    protected bool CantSave => string.IsNullOrWhiteSpace(DocumentPage.Name) || (DocumentPage.Id > 0 && DocumentPage.Name == NameOrigin && DocumentPage.Description == DescriptionOrigin && DocumentPage.Description == DescriptionOrigin);
    bool IsEdited => DocumentPage.Name != NameOrigin || DocumentPage.Description != DescriptionOrigin;

    /// <summary>
    /// Перемещение страницы опроса/анкеты (сортировка страниц внутри опроса/анкеты)
    /// </summary>
    protected async Task MoveRow(VerticalDirectionsEnum direct)
    {
        if (CurrentUser is null)
            throw new Exception("CurrentUser is null");

        IsBusyProgress = true;
        TResponseModel<DocumentSchemeConstructorModelDB> rest = await ConstructorRepo.MoveTabOfDocumentScheme(DocumentPage.Id, direct);
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

        UpdateDocumentHandle(rest.Response, DocumentPage);
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
        ResponseBaseModel rest = await ConstructorRepo.DeleteTabOfDocumentScheme(DocumentPage.Id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
            return;
        }
        DocumentReloadHandle();
    }

    /// <summary>
    /// Отмена редактирования
    /// </summary>
    protected void CancelEditing()
    {
        IsInitDelete = false;
        DocumentPage.Name = NameOrigin ?? "";
        DocumentPage.Description = DescriptionOrigin;
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
        ResponseBaseModel rest = await ConstructorRepo.CreateOrUpdateTabDocumentSchemeJoinForm(new TabJoinDocumentSchemeConstructorModelDB()
        {
            FormId = SelectedFormForAdding,
            OwnerId = DocumentPage.Id,
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
        TabOfDocumentSchemeResponseModel rest = await ConstructorRepo.CreateOrUpdateTabOfDocumentScheme(new EntryDescriptionOwnedModel() { Id = DocumentPage.Id, OwnerId = DocumentPage.OwnerId, Name = DocumentPage.Name, Description = DocumentPage.Description });
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
            SnackbarRepo.Add($"Ошибка 07653445-0B30-46CB-9B79-3B068BAB9AEB rest.Content.DocumentPage is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        int i = DocumentPage.Id;
        DocumentPage.Id = rest.TabOfDocumentScheme.Id;
        SetIdForPageHandle(i, rest.TabOfDocumentScheme);

        DescriptionOrigin = Description;
        NameOrigin = Name;
        IsInitDelete = false;

        SetHoldHandle(IsEdited);
    }

    async Task ReloadPage()
    {
        if (DocumentPage.Id < 1)
            return;

        IsBusyProgress = true;
        TabOfDocumentSchemeResponseModel rest = await ConstructorRepo.GetTabOfDocumentScheme(DocumentPage.Id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка 815BCE17-9180-4C27-8016-BEB5244A3454 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.TabOfDocumentScheme is null)
        {
            SnackbarRepo.Add($"Ошибка 5B879025-EC6E-4989-9A75-5844BD20DF0B Content [rest.Content.DocumentPage is null]", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        DocumentPage.JoinsForms = rest.TabOfDocumentScheme?.JoinsForms;
        DocumentPage.Owner = rest.TabOfDocumentScheme?.Owner;
        SetIdForPageHandle(DocumentPage.Id, DocumentPage);
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReloadPage();
        NameOrigin = DocumentPage.Name;
        DescriptionOrigin = DocumentPage.Description;
    }
}