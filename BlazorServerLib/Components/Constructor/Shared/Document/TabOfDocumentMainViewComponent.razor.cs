////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.Document;

/// <summary>
/// Page questionnaire form main view
/// </summary>
public partial class TabOfDocumentMainViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;

    [Inject]
    ILogger<TabOfDocumentMainViewComponent> LoggerRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public required TabOfDocumentSchemeConstructorModelDB DocumentPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormToTabJoinConstructorModelDB PageJoinForm { get; set; }

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
    public required bool InUse { get; set; }


    UserInfoMainModel? user;

    /// <inheritdoc/>
    protected async Task DeleteJoinForm()
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        IsBusyProgress = true;
        await Task.Delay(1);
        ResponseBaseModel rest = await ConstructorRepo.DeleteTabDocumentSchemeJoinForm(new() { Payload = PageJoinForm.Id, SenderActionUserId = user.UserId });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
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

    string? _join_name_origin;
    string? PageJoinFormName
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
    protected async Task DocumentPageJoinFormMove(VerticalDirectionsEnum direct)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<TabOfDocumentSchemeConstructorModelDB> rest = await ConstructorRepo.MoveTabDocumentSchemeJoinForm(new() { Payload = new() { Id = PageJoinForm.Id, Direct = direct }, SenderActionUserId = user.UserId });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
            return;
        }
        UpdatePageActionHandle(null);
    }

    /// <inheritdoc/>
    protected async Task SaveJoinForm()
    {
        if(user is null)
            throw new ArgumentNullException(nameof(user));

        FormToTabJoinConstructorModelDB req = new()
        {
            Description = PageJoinForm.Description,
            FormId = PageJoinForm.FormId,
            Id = PageJoinForm.Id,
            IsTable = PageJoinForm.IsTable,
            Name = PageJoinForm.Name,
            TabId = PageJoinForm.TabId,
            ShowTitle = PageJoinForm.ShowTitle,
            SortIndex = PageJoinForm.SortIndex
        };

        IsBusyProgress = true;
        await Task.Delay(1);
        ResponseBaseModel rest = await ConstructorRepo.CreateOrUpdateTabDocumentSchemeJoinForm(new() { Payload = req, SenderActionUserId = user.UserId });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
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
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo();

        _join_name_origin = PageJoinForm.Name;
        _join_set_title_origin = PageJoinForm.ShowTitle == true;
        _is_table_origin = PageJoinForm.IsTable;

        if (PageJoinForm.Form is null)
        {
            LoggerRepo.LogWarning("Дозагрузка [Form] для [PageJoinForm]...");
            IsBusyProgress = true;
            TResponseModel<FormConstructorModelDB> rest = await ConstructorRepo.GetForm(PageJoinForm.FormId);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            PageJoinForm.Form = rest.Response;
        }
    }
}