////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Helpdesk;
using Microsoft.AspNetCore.Components;
using SharedLib;
using BlazorLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// TabAddressOfOrderDocumentComponent
/// </summary>
public partial class TabAddressOfOrderDocumentComponent : OffersTableBaseComponent
{
    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    /// <summary>
    /// CurrentTab
    /// </summary>
    [Parameter, EditorRequired]
    public required TabAddressForOrderModelDb CurrentTab { get; set; }


    /// <summary>
    /// Если true - тогда можно добавлять офферы, которых нет в остатках.
    /// Если false - тогда для добавления доступны только офферы на остатках
    /// </summary>
    [Parameter]
    public bool ForceAdding { get; set; }


    AddRowToOrderDocumentComponent? addingDomRef;
    RowOfOrderDocumentModelDB? elementBeforeEdit;
    RubricSelectorComponent? ref_rubric;
    List<RubricIssueHelpdeskModelDB>? RubricMetadataShadow;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await SetBusy();
        TResponseModel<List<RubricIssueHelpdeskModelDB>?> res = await HelpdeskRepo.RubricRead(0);
        await CacheRegistersGoodsUpdate(CurrentTab.Rows!.Select(x => x.OfferId), CurrentTab.WarehouseId, true);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        RubricMetadataShadow = res.Response;
        if (RubricMetadataShadow is not null && RubricMetadataShadow.Count != 0)
        {
            RubricIssueHelpdeskModelDB current_element = RubricMetadataShadow.Last();
            if (ref_rubric is not null)
            {
                await ref_rubric.OwnerRubricSet(current_element.ParentRubricId ?? 0);
                await ref_rubric.SetRubric(current_element.Id, RubricMetadataShadow);
                ref_rubric.StateHasChangedCall();
            }
        }
    }

    int GetMaxValue(RowOfOrderDocumentModelDB ctx)
    {
        return ForceAdding
            ? int.MaxValue
            : RegistersCache.Where(x => x.OfferId == ctx.OfferId && x.WarehouseId == CurrentTab.WarehouseId).Sum(x => x.Quantity);
    }

    void RubricSelectAction(RubricBaseModel? selectedRubric)
    {
        CurrentTab.WarehouseId = selectedRubric?.Id ?? 0;

        if (DocumentUpdateHandler is not null)
            DocumentUpdateHandler();

        if (CurrentTab.Rows is not null)
            InvokeAsync(async () =>
            {
                await CacheRegistersGoodsUpdate(CurrentTab.Rows.Select(x => x.OfferId), CurrentTab.WarehouseId, true);
                StateHasChanged();
            });
        else
            StateHasChanged();
    }

    /// <inheritdoc/>
    protected override void DeleteRow(int offerId)
    {
        CurrentTab.Rows ??= [];
        CurrentTab.Rows.RemoveAll(x => x.OfferId == offerId);
        if (DocumentUpdateHandler is not null)
            DocumentUpdateHandler();

        addingDomRef?.StateHasChangedCall();
    }

    /// <inheritdoc/>
    protected override void AddingOfferAction(OfferGoodActionModel off)
    {
        CurrentTab.Rows ??= [];
        int exist_row = CurrentTab.Rows.FindIndex(x => x.OfferId == off.Id);
        if (exist_row < 0)
            CurrentTab.Rows.Add(new RowOfOrderDocumentModelDB()
            {
                AddressForOrderTab = CurrentTab,
                AddressForOrderTabId = CurrentTab.Id,
                Goods = off.Goods,
                GoodsId = off.GoodsId,
                Offer = off,
                OfferId = off.Id,
                OrderDocument = CurrentTab.OrderDocument,
                OrderDocumentId = CurrentTab.OrderDocumentId,
                Quantity = off.Quantity,
            });
        else
            CurrentTab.Rows[exist_row].Quantity = +off.Quantity;

        if (DocumentUpdateHandler is not null)
            DocumentUpdateHandler();

        StateHasChanged();
        addingDomRef!.StateHasChangedCall();
    }

    /// <inheritdoc/>
    protected override void RowEditPreviewHandler(object element)
        => elementBeforeEdit = GlobalTools.CreateDeepCopy((RowOfOrderDocumentModelDB)element);

    /// <inheritdoc/>
    protected override void RowEditCancelHandler(object element)
    {
        ((RowOfOrderDocumentModelDB)element).Quantity = elementBeforeEdit!.Quantity;
        elementBeforeEdit = null;
    }
}