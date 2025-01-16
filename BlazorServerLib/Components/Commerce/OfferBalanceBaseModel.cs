////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OfferBalanceBaseModel
/// </summary>
public class OfferBalanceBaseModel : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// 
    /// </summary>
    [Parameter, EditorRequired]
    public required OfferModelDB ContextOffer { get; set; }

    /// <summary>
    /// Parent
    /// </summary>
    [Parameter, EditorRequired]
    public required OffersComponent Parent { get; set; }
}