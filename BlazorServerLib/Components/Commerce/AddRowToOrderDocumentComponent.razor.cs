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
    public required AddressForOrderModelDb CurrentTab { get; set; }

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

    IEnumerable<OfferGoodModelDB> ActualOffers => AllOffers.Where(x => !CurrentTab.Rows!.Any(y => y.OfferId == x.Id));

    bool _expanded;
    int QuantityValue { get; set; } = 1;
    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }

    void AddOffer()
    {
        QuantityValue = 1;
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
        AllOffers.RemoveAll(x => x.Id == SelectedOffer?.Id);

        SelectedOffer = ActualOffers.FirstOrDefault();
        _selectedOfferId = SelectedOffer?.Id;
        OnExpandCollapseClick();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        SelectedOfferId = ActualOffers.FirstOrDefault()?.Id;
    }
}