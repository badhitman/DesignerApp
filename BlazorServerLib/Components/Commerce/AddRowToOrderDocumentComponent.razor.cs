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
public partial class AddRowToOrderDocumentComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// AllOffers
    /// </summary>
    [Parameter, EditorRequired]
    public required List<OfferGoodModelDB> AllOffers { get; set; }

    /// <summary>
    /// CurrentTab
    /// </summary>
    [Parameter, EditorRequired]
    public required List<int> CurrentRows { get; set; }

    /// <summary>
    /// AddingOfferHandler
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<OfferGoodActionModel> AddingOfferHandler { get; set; }


    OfferGoodModelDB? SelectedOffer { get; set; }

    int? _selectedOfferId;
    int? SelectedOfferId
    {
        get => _selectedOfferId;
        set
        {
            _selectedOfferId = value;
            SelectedOffer = AllOffers.FirstOrDefault(x => x.Id == value);
        }
    }

    IEnumerable<OfferGoodModelDB> ActualOffers => AllOffers.Where(x => !CurrentRows!.Contains(x.Id));

    bool IsShowAddingOffer;
    int QuantityValue { get; set; } = 1;
    private void OnExpandAddingOffer()
    {
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

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        SelectedOfferId = ActualOffers.FirstOrDefault()?.Id;
    }
}