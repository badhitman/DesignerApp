////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// PricesRulesForOfferComponent
/// </summary>
public partial class PricesRulesForOfferComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ILogger<PricesRulesForOfferComponent> LoggerRepo { get; set; } = default!;


    /// <summary>
    /// Offer
    /// </summary>
    [Parameter, EditorRequired]
    public required OfferModelDB Offer { get; set; }


    bool IsExpandPanel;
    List<PriceRuleForOfferModelDB> rules = default!;

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
        if (CurrentUserSession is null)
            return;

        await SetBusy();
        TResponseModel<int> res = await CommerceRepo.PriceRuleUpdate(new() { Payload = rule, SenderActionUserId = CurrentUserSession.UserId });
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        LoggerRepo.LogWarning("Правило изменено");
        await ReloadRules();
    }

    /// <inheritdoc/>
    public async Task ReloadRules()
    {
        if (CurrentUserSession is null)
            return;

        QuantityAddingRule = 2;
        PriceAddingRule = 0;
        //
        await SetBusy();
        TResponseModel<List<PriceRuleForOfferModelDB>> res = await CommerceRepo.PricesRulesGetForOffers(new() { Payload = [Offer.Id], SenderActionUserId = CurrentUserSession.UserId });
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Response is not null && res.Success())
            rules = res.Response;
        IsBusyProgress = false;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReadCurrentUser();
        await ReloadRules();
    }
}