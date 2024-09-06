////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
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


    OfferGoodModelDB? selectedOffer { get; set; }

    int? _selectedOfferId;
    int? SelectedOfferId
    {
        get => _selectedOfferId;
        set
        {
            _selectedOfferId = value;
            selectedOffer = AllOffers.FirstOrDefault(x => x.Id == value);
        }
    }


    bool _expanded;
    int IntValue { get; set; } = 1;
    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }
}