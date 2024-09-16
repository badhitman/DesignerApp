////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using Microsoft.Extensions.Logging;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// PricesRulesForOfferComponent
/// </summary>
public partial class PricesRulesForOfferComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ILogger<PricesRulesForOfferComponent> LoggerRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// OfferGood
    /// </summary>
    [Parameter, EditorRequired]
    public required OfferGoodModelDB OfferGood { get; set; }


    bool IsExpandPanel;
    PriceRuleForOfferModelDB[] rules = default!;
    
    int QuantityAddingRule { get; set; } = 2;
    decimal PriceAddingRule { get; set; }

    string? TextValue { get; set; }
    

    /// <inheritdoc/>
    public List<PriceRuleElementComponent> RulesViewsComponents { get; set; } = [];


    /// <inheritdoc/>
    public void SetActive(int rule_id)
    {
        RulesViewsComponents.ForEach(component => component.IsActive = component.PriceRule.Id == rule_id);
        QuantityAddingRule = 2;
        PriceAddingRule = 0;
        StateHasChanged();
    }

    /// <inheritdoc/>
    public async Task SaveRule(PriceRuleForOfferModelDB rule)
    {
        IsBusyProgress = true;
        TResponseModel<int> res = await CommerceRepo.PriceRuleUpdate(rule);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        LoggerRepo.LogWarning("Правило изменено");
        await ReloadRules();
    }

    /// <inheritdoc/>
    public async Task ReloadRules()
    {
        QuantityAddingRule = 2;
        PriceAddingRule = 0;
        //
        IsBusyProgress = true;
        TResponseModel<PriceRuleForOfferModelDB[]> res = await CommerceRepo.PricesRulesGetForOffers([OfferGood.Id]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        rules = res.Response ?? throw new Exception();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReloadRules();
    }
}