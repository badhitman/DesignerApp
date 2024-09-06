﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OfferCardEditComponent
/// </summary>
public partial class OfferCardEditComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// OfferId
    /// </summary>
    [Parameter, EditorRequired]
    public int OfferId { get; set; }


    OfferGoodModelDB CurrentOffer = default!;
    OfferGoodModelDB editOffer = default!;

    bool CanSave =>
        editOffer.IsDisabled != CurrentOffer.IsDisabled ||
        editOffer.Name != CurrentOffer.Name || 
        editOffer.Price != CurrentOffer.Price || 
        editOffer.Multiplicity != CurrentOffer.Multiplicity || 
        editOffer.OfferUnit != CurrentOffer.OfferUnit;

    async Task SaveOffer()
    {
        IsBusyProgress = true;
        TResponseModel<int?> res = await CommerceRepo.OfferUpdate(editOffer);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success() && res.Response.HasValue)
            CurrentOffer = GlobalTools.CreateDeepCopy(editOffer)!;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<OfferGoodModelDB[]?> res = await CommerceRepo.OffersRead([OfferId]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        CurrentOffer = res.Response!.Single();
        editOffer = GlobalTools.CreateDeepCopy(CurrentOffer)!;
    }
}