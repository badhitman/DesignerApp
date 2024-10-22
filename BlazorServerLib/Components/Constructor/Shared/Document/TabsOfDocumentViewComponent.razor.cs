////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.Document;

/// <summary>
/// Page questionnaire forms - view
/// </summary>
public partial class TabsOfDocumentViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;


    /// <summary>
    /// DocumentScheme page
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabOfDocumentSchemeConstructorModelDB TabOfDocumentScheme { get; set; }


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
            TabOfDocumentScheme = page;
            StateHasChanged();
            return;
        }
        SetBusy();
        _ = InvokeAsync(async () =>
        {
            TResponseModel<TabOfDocumentSchemeConstructorModelDB> rest = await ConstructorRepo.GetTabOfDocumentScheme(TabOfDocumentScheme.Id);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            if (!rest.Success())
            {
                SnackbarRepo.Add($"Ошибка 16188CA3-EC20-4743-A31C-DA497CABDEB5 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            if (rest.Response is null)
            {
                SnackbarRepo.Add($"Ошибка E7427B3A-68CB-4560-B2E0-4AF69F2EDA72 [rest.Content.DocumentPage is null]", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            TabOfDocumentScheme = rest.Response;
            TabOfDocumentScheme.JoinsForms = TabOfDocumentScheme.JoinsForms?.OrderBy(x => x.SortIndex).ToList();
            StateHasChanged();
        });
    }

    /// <summary>
    /// Форму можно сдвинуть выше?
    /// </summary>
    protected bool CanUpJoinForm(FormToTabJoinConstructorModelDB pjf)
    {
        int min_index = TabOfDocumentScheme.JoinsForms?.Any(x => x.Id != pjf.Id) == true
        ? TabOfDocumentScheme.JoinsForms.Where(x => x.Id != pjf.Id).Min(x => x.SortIndex)
        : 1;
        return _join_form_id == 0 && pjf.SortIndex > min_index;
    }

    /// <summary>
    /// Форму можно сдвинуть ниже?
    /// </summary>
    protected bool CanDownJoinForm(FormToTabJoinConstructorModelDB pjf)
    {
        int max_index = TabOfDocumentScheme.JoinsForms?.Any(x => x.Id != pjf.Id) == true
        ? TabOfDocumentScheme.JoinsForms.Where(x => x.Id != pjf.Id).Max(x => x.SortIndex)
        : 1;
        return _join_form_id == 0 && pjf.SortIndex < max_index;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (TabOfDocumentScheme.JoinsForms is null)
        {
            SnackbarRepo.Add($"Дозагрузка `{nameof(TabOfDocumentScheme.JoinsForms)}` в `{nameof(TabOfDocumentScheme)} ['{TabOfDocumentScheme.Name}' #{TabOfDocumentScheme.Id}]`", Severity.Info, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            SetBusy();
            TResponseModel<TabOfDocumentSchemeConstructorModelDB> rest = await ConstructorRepo.GetTabOfDocumentScheme(TabOfDocumentScheme.Id);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            TabOfDocumentScheme.JoinsForms = rest.Response?.JoinsForms;
        }
    }
}