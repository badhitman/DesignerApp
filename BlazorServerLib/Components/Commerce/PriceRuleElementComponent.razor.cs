////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// PriceRuleElementComponent
/// </summary>
public partial class PriceRuleElementComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// PriceRule
    /// </summary>
    [Parameter, EditorRequired]
    public required PriceRuleForOfferModelDB PriceRule { get; set; }

    /// <summary>
    /// OwnerComponent
    /// </summary>
    [Parameter, EditorRequired]
    public required PricesRulesForOfferComponent OwnerComponent { get; set; }


    bool InitDelete;

    /// <inheritdoc/>
    public bool IsActive { get; set; }


    /// <inheritdoc/>
    public async Task SaveRule()
    {
        SetBusy();
        
        TResponseModel<int> res = await CommerceRepo.PriceRuleUpdate(PriceRule);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await OwnerComponent.ReloadRules();
    }

    /// <inheritdoc/>
    public async Task DeleteRule()
    {
        SetBusy();
        
        TResponseModel<bool> res = await CommerceRepo.PriceRuleDelete(PriceRule.Id);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await OwnerComponent.ReloadRules();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        OwnerComponent.RulesViewsComponents.Add(this);
    }
}