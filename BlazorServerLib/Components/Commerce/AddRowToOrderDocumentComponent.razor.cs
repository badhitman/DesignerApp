////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// AddRowToOrderDocumentComponent
/// </summary>
public partial class AddRowToOrderDocumentComponent : BlazorBusyComponentRegistersModel
{
    /// <summary>
    /// Склад
    /// </summary>
    [Parameter, EditorRequired]
    public required int WarehouseId { get; set; }

    /// <summary>
    /// Все офферы
    /// </summary>
    [Parameter, EditorRequired]
    public required List<OfferGoodModelDB> AllOffers { get; set; }

    /// <summary>
    /// Текущие/выбранные строки
    /// </summary>
    [Parameter, EditorRequired]
    public required List<int> CurrentRows { get; set; }

    /// <summary>
    /// Если true - тогда отображается цена за единицу (непосредственно в элементах html селектора/select: option).
    /// Если false - тогда цена за единицу не будет отображаться
    /// </summary>
    [Parameter]
    public bool ShowPriceOffers { get; set; }

    /// <summary>
    /// Если true - тогда отображается остаток (непосредственно в элементах html селектора/select: option).
    /// Если false - тогда остаток не будет отображаться
    /// </summary>
    [Parameter]
    public bool ShowRegistersOffersQuantity { get; set; }

    /// <summary>
    /// Если true - тогда можно добавлять офферы, которых нет в остатках.
    /// Если false - тогда для добавления доступны только офферы на остатках
    /// </summary>
    [Parameter]
    public bool ForceAdding { get; set; }

    /// <summary>
    /// Обработчик добавления оффера
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<OfferGoodActionModel> AddingOfferHandler { get; set; }

    /// <summary>
    /// Обработчик выбора Оффера
    /// </summary>
    [Parameter]
    public Action<OfferGoodModelDB?>? SelectOfferHandler { get; set; }


    OfferGoodModelDB? SelectedOffer { get; set; }

    int? _selectedOfferId;
    /// <summary>
    /// SelectedOfferId
    /// </summary>
    public int? SelectedOfferId
    {
        get => _selectedOfferId;
        set
        {
            _selectedOfferId = value;
            SelectedOffer = AllOffers.FirstOrDefault(x => x.Id == value);

            System.Collections.Immutable.ImmutableList<decimal>? _qv = SelectedOffer?.QuantitiesValues;
            if (_qv is not null && _qv.Count != 0 && !ForceAdding)
                QuantityValue = _qv.First();
            else
                QuantityValue = 1;

            if (SelectedOffer is not null && !ForceAdding)
                InvokeAsync(async () => await CacheRegistersOfferUpdate([SelectedOffer.Id], WarehouseId, true));
            if (SelectOfferHandler is not null)
                SelectOfferHandler(SelectedOffer);
        }
    }

    decimal GetOfferQuantity(OfferGoodModelDB opt)
    {

        return RegistersCache.Where(x => x.OfferId == opt.Id && (WarehouseId < 1 || x.WarehouseId == WarehouseId)).Sum(x => x.Quantity);
    }

    decimal GetMaxValue()
    {
        if (ForceAdding)
            return decimal.MaxValue;

        return SelectedOffer is null
            ? 0
            : RegistersCache.Where(x => x.OfferId == SelectedOffer.Id && x.WarehouseId == WarehouseId).Sum(x => x.Quantity);
    }

    bool CantAdd()
    {
        return !ForceAdding && (SelectedOffer is null || GetMaxValue() == 0);
    }

    IEnumerable<OfferGoodModelDB> ActualOffers => AllOffers.Where(x => !CurrentRows!.Contains(x.Id));

    bool IsShowAddingOffer;
    decimal QuantityValue { get; set; }
    private void OnExpandAddingOffer()
    {
        System.Collections.Immutable.ImmutableList<decimal>? _qv = SelectedOffer?.QuantitiesValues;
        if (!IsShowAddingOffer && _qv is not null && _qv.Count != 0 && !ForceAdding)
            QuantityValue = _qv.First();
        else
            QuantityValue = 1;

        IsShowAddingOffer = !IsShowAddingOffer;
        if (IsShowAddingOffer)
        {
            SelectedOffer = ActualOffers.FirstOrDefault();
            _selectedOfferId = SelectedOffer?.Id;
        }
    }

    IEnumerable<IGrouping<GoodsModelDB?, OfferGoodModelDB>> OffersNodes => ActualOffers.GroupBy(x => x.Goods);

    void AddOffer()
    {
        AddingOfferHandler(new OfferGoodActionModel()
        {
            Name = SelectedOffer!.Name,
            Goods = SelectedOffer.Goods,
            Quantity = QuantityValue,
            Multiplicity = SelectedOffer.Multiplicity,
            OfferUnit = SelectedOffer.OfferUnit,
            Price = SelectedOffer.Price,
            IsDisabled = SelectedOffer.IsDisabled,
            GoodsId = SelectedOffer.GoodsId,
            Id = SelectedOffer.Id,
        });
        QuantityValue = 1;

        SelectedOffer = ActualOffers.FirstOrDefault();
        _selectedOfferId = SelectedOffer?.Id;
        OnExpandAddingOffer();
    }

    int? cacheId = null;

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            if (cacheId != SelectedOfferId && SelectedOffer is not null && !ForceAdding)
            {
                await CacheRegistersOfferUpdate([], WarehouseId, true);
                cacheId = SelectedOfferId;
                StateHasChanged();
            }
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        SelectedOfferId = ActualOffers.FirstOrDefault()?.Id;
        if (SelectedOffer is not null && !ForceAdding)
            await CacheRegistersOfferUpdate([SelectedOffer.Id], WarehouseId, true);

        cacheId = SelectedOfferId;
    }
}