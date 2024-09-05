////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OfferCreatingFormComponent
/// </summary>
public partial class OfferCreatingFormComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// CurrentGoods
    /// </summary>
    [Parameter, EditorRequired]
    public required GoodsModelDB CurrentGoods { get; set; }

    /// <summary>
    /// OfferCreatingHandler
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<OfferGoodModelDB> OfferCreatingHandler { get; set; }


    UnitsOfMeasurementEnum UnitOffer { get; set; } = UnitsOfMeasurementEnum.None;
    double priceOffer;
    uint multiplicityOffer;
    string? nameOffer;

    bool CanSave => priceOffer > 0 && multiplicityOffer > 0 && UnitOffer != UnitsOfMeasurementEnum.None;

    async Task AddOffer()
    {
        OfferGoodModelDB off = new()
        {
            Name = nameOffer ?? "",
            GoodsId = CurrentGoods.Id,
            Multiplicity = multiplicityOffer,
            OfferUnit = UnitOffer,
            Price = priceOffer,
        };
        IsBusyProgress = true;
        TResponseModel<int?> res = await CommerceRepo.OfferUpdate(off);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success() && res.Response.HasValue)
        {
            off.Id = res.Response.Value;
            OfferCreatingHandler(off);

            UnitOffer = UnitsOfMeasurementEnum.None;
            priceOffer = 0;
            multiplicityOffer = 0;
            nameOffer = null;
        }
    }
}