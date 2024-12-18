////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OrderCreateComponent
/// </summary>
public partial class OrderCreateComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StorageRepo { get; set; } = default!;

    [Inject]
    NavigationManager NavRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    bool _visibleChangeAddresses;
    bool _visibleChangeOrganization;
    readonly DialogOptions _dialogOptions = new() { FullWidth = true };

    OrderDocumentModelDB? CurrentCart;
    readonly Func<AddressOrganizationModelDB, string> converter = p => p.Name;

    List<OrganizationModelDB> Organizations { get; set; } = [];
    OrganizationModelDB? prevCurrOrg;
    OrganizationModelDB? CurrentOrganization
    {
        get => CurrentCart?.Organization;
        set
        {
            if (SelectedAddresses?.Any() == true)
            {
                _visibleChangeOrganization = true;
                prevCurrOrg = value;
                return;
            }

            if (CurrentCart is null)
#pragma warning disable CA2208
                throw new ArgumentNullException(nameof(CurrentCart), GetType().FullName);
#pragma warning restore CA2208

            CurrentCart.Organization = value;
            CurrentCart.OrganizationId = value?.Id ?? 0;
            ResetAddresses();
        }
    }

    /// <summary>
    /// Предварительный набор адресов, который пользователь должен утвердить/подтвердить.
    /// </summary>
    IEnumerable<AddressOrganizationModelDB>? _prevSelectedAddresses;
    List<AddressOrganizationModelDB>? _selectedAddresses = [];
    IEnumerable<AddressOrganizationModelDB>? SelectedAddresses
    {
        get => _selectedAddresses ?? [];
        set
        {
            if (_prevSelectedAddresses is not null || CurrentCart is null)
                return;

            CurrentCart.AddressesTabs ??= [];

            // адреса/вкладки, которые следует добавить
            AddressOrganizationModelDB[] addresses = value is null ? [] : [.. value.Where(x => !CurrentCart.AddressesTabs.Any(y => y.AddressOrganizationId == x.Id))];
            if (addresses.Length != 0)
            {
                CurrentCart.AddressesTabs.AddRange(addresses.Select(x => new TabAddressForOrderModelDb()
                {
                    AddressOrganizationId = x.Id,
                    Rows = [],
                    OrderDocument = CurrentCart,
                    OrderDocumentId = CurrentCart.Id,
                    AddressOrganization = new()
                    {
                        Id = x.Id,
                        Address = x.Address,
                        Name = x.Name,
                        ParentId = x.ParentId,
                        Contacts = x.Contacts,
                        OrganizationId = CurrentOrganization!.Id,
                        Organization = CurrentOrganization,
                    }
                }));
                _selectedAddresses = [.. CurrentCart.AddressesTabs.Select(Convert)];
                InvokeAsync(async () => await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(CurrentUserSession!.UserId), false));
            }
            static AddressOrganizationModelDB Convert(TabAddressForOrderModelDb x) => new()
            {
                Id = x.AddressOrganization!.Id,
                Name = x.AddressOrganization.Name,
                Address = x.AddressOrganization.Address,
                Contacts = x.AddressOrganization.Contacts,
                ParentId = x.AddressOrganization.ParentId,
                Organization = x.AddressOrganization.Organization,
                OrganizationId = x.AddressOrganization.OrganizationId,
            };

            // адреса/вкладки, которые пользователь хочет удалить
            TabAddressForOrderModelDb[] _qr = CurrentCart
                .AddressesTabs
                .Where(x => value?.Any(y => y.Id == x.AddressOrganizationId) != true).ToArray();

            // адреса/вкладки, которые можно свободно удалить (без строк)
            AddressOrganizationModelDB[] _prev = _qr
                 .Where(x => x.Rows is null || x.Rows.Count == 0)
                 .Select(Convert)
                 .ToArray();
            if (_prev.Length != 0)
            {
                CurrentCart.AddressesTabs!.RemoveAll(x => _prev.Any(y => y.Id == x.AddressOrganizationId));
                _selectedAddresses = [.. CurrentCart.AddressesTabs.Select(Convert)];
                InvokeAsync(async () => { await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(CurrentUserSession!.UserId), true); });
            }

            // адреса/вкладки, которые имеют строки: требуют подтверждения у пользователя
            _prev = _qr
                .Where(x => x.Rows is not null && x.Rows.Count != 0)
                .Select(Convert)
                .ToArray();

            if (_prev.Length != 0)
            {
                _prevSelectedAddresses = value;
                _visibleChangeAddresses = true;
            }
            else
            {
                _prevSelectedAddresses = null;
                _selectedAddresses = [.. value];
            }
        }
    }

    RowOfOrderDocumentModelDB[] AllRows = default!;

    /// <summary>
    /// Сгруппировано по OfferId
    /// </summary>
    List<IGrouping<int, RowOfOrderDocumentModelDB>> GroupingRows { get; set; } = default!;

    private readonly Dictionary<int, PriceRuleForOfferModelDB[]?> RulesCache = [];
    readonly Dictionary<int, decimal> DiscountsDetected = [];

    async Task ActualityData()
    {
        int[]? offersIds = CurrentCart?
            .AddressesTabs?
            .SkipWhile(x => x.Rows is null)
            .SelectMany(x => x.Rows!.Select(y => y.OfferId))
            .Distinct()
            .ToArray();

        if (offersIds is null || offersIds.Length == 0)
            return;

        await SetBusy();
        TResponseModel<OfferModelDB[]> offersRes = await CommerceRepo.OffersRead(offersIds);
        if (!offersRes.Success() || offersRes.Response is null || offersRes.Response.Length == 0)
        {
            SnackbarRepo.ShowMessagesResponse(offersRes.Messages);
            return;
        }

        CurrentCart!.AddressesTabs!.ForEach(adRow =>
        {
            adRow.Rows?.ForEach(orderRow =>
                {
                    orderRow.Offer = offersRes.Response.First(sr => sr.Id == orderRow.OfferId);
                });
        });

        await SetBusy(false);
    }

    async Task UpdateCachePriceRules()
    {
        CurrentCart ??= OrderDocumentModelDB.NewEmpty(CurrentUserSession!.UserId);

        if (CurrentCart.AddressesTabs is null)
        {
            AllRows = [];
            GroupingRows = [];
            return;
        }

        AllRows = [.. CurrentCart.AddressesTabs?.Where(x => x.Rows is not null).SelectMany(x => x.Rows!)];
        GroupingRows = AllRows.GroupBy(x => x.OfferId).ToList();
        List<int> offers_load = [.. GroupingRows.Where(dc => !RulesCache.ContainsKey(dc.Key)).Select(x => x.Key).Distinct()];

        if (offers_load.Count == 0)
            return;

        await SetBusy();

        TResponseModel<PriceRuleForOfferModelDB[]> res = await CommerceRepo.PricesRulesGetForOffers([.. offers_load]);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        offers_load.ForEach(x =>
        {
            if (RulesCache.ContainsKey(x))
                RulesCache[x] = res.Response?.Where(y => x == y.OfferId && !y.IsDisabled).ToArray();
            else
                RulesCache.Add(x, res.Response?.Where(y => x == y.OfferId && !y.IsDisabled).ToArray());
        });
    }

    void CalculateDiscounts()
    {
        if (GroupingRows.Any(x => x.Any(y => y.Offer is null)))
            return;

        string json_dump_discounts_before = JsonConvert.SerializeObject(DiscountsDetected);
        DiscountsDetected.Clear();
        foreach (IGrouping<int, RowOfOrderDocumentModelDB> node in GroupingRows)
        {
            decimal qnt = node.Sum(y => y.Quantity); // всего количество в заказе
            if (qnt <= 1 || !RulesCache.TryGetValue(node.Key, out PriceRuleForOfferModelDB[]? _rules) || _rules?.Any() != true)
                continue;

            decimal base_price = node.First().Offer!.Price;
            PriceRuleForOfferModelDB? find_rule = null;
            DiscountsDetected.Add(node.Key, 0);
            for (int i = 2; i <= qnt; i++)
            {
                find_rule = _rules.FirstOrDefault(x => x.QuantityRule == i) ?? find_rule;
                if (find_rule is null)
                    continue;

                DiscountsDetected[node.Key] += base_price - find_rule.PriceRule;
            }

            if (DiscountsDetected[node.Key] == 0)
                DiscountsDetected.Remove(node.Key);
        }

        string json_dump_discounts_after = JsonConvert.SerializeObject(DiscountsDetected);
        if (json_dump_discounts_before != json_dump_discounts_after)
            StateHasChanged();
    }

    decimal SumOfDocument
    {
        get
        {
            if (CurrentCart is null)
                throw new ArgumentNullException(nameof(CurrentCart), GetType().FullName);

            if (CurrentCart.AddressesTabs is null || CurrentCart.AddressesTabs.Count == 0)
                return 0;

            IQueryable<RowOfOrderDocumentModelDB> rows = CurrentCart
                .AddressesTabs
                .Where(x => x.Rows is not null)
                .SelectMany(x => x.Rows!)
                .AsQueryable();

            if (!rows.Any())
                return 0;

            return rows.Sum(x => x.Quantity * x.Offer!.Price);
        }
    }

    async Task SubmitChangeAddresses()
    {
        if (CurrentCart is null)
            throw new ArgumentNullException(nameof(CurrentCart), GetType().FullName);

        CurrentCart.AddressesTabs ??= [];
        if (_prevSelectedAddresses is null || !_prevSelectedAddresses.Any())
            CurrentCart.AddressesTabs.Clear();
        else
            CurrentCart.AddressesTabs.RemoveAll(x => !_prevSelectedAddresses.Any(y => y.Id == x.AddressOrganizationId));

        _selectedAddresses = [.. _prevSelectedAddresses];
        _prevSelectedAddresses = null;
        _visibleChangeAddresses = false;
        await SetBusy();

        await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(CurrentUserSession!.UserId), true);
        await SetBusy(false);
    }

    void CancelChangeAddresses()
    {
        _visibleChangeAddresses = false;
        _prevSelectedAddresses = null;
    }

    void SubmitChangeOrganizations()
    {
        if (CurrentCart is null)
            throw new ArgumentNullException(nameof(CurrentCart), GetType().FullName);

        CurrentCart.Organization = prevCurrOrg;
        CurrentCart.OrganizationId = prevCurrOrg?.Id ?? 0;
        prevCurrOrg = null;
        CurrentCart.AddressesTabs?.RemoveAll(x => !CurrentOrganization!.Addresses!.Any(y => y.Id == x.AddressOrganizationId));
        ResetAddresses();
        _visibleChangeOrganization = false;
    }

    async Task ClearOrder()
    {
        if (CurrentCart?.AddressesTabs is null)
            return;
        CurrentCart.AddressesTabs.ForEach(x => x.Rows?.Clear());
        await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(CurrentUserSession!.UserId), true);
        NavRepo.Refresh(true);
    }

    async void DocumentUpdateAction()
    {
        if (CurrentCart is null)
            throw new ArgumentNullException(nameof(CurrentCart), GetType().FullName);

        await SetBusy();
        CurrentCart.AddressesTabs?.RemoveAll(x => !CurrentOrganization!.Addresses!.Any(y => y.Id == x.AddressOrganizationId));
        await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(CurrentUserSession!.UserId), true);
        await SetBusy(false);
    }

    void CancelChangeOrganizations()
    {
        _visibleChangeOrganization = false;
        prevCurrOrg = null;
    }

    void ResetAddresses()
    {
        _selectedAddresses?.Clear();
        InvokeAsync(async () => { await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(CurrentUserSession!.UserId), true); });
    }

    async Task OrderDocumentSendAsync()
    {
        if (CurrentCart is null)
            throw new ArgumentNullException(nameof(CurrentCart), GetType().FullName);

        if (CurrentCart.AddressesTabs?.Any(x => x.Rows is null || x.Rows.Count == 0) == true)
        {
            SnackbarRepo.Error("Присутствуют адреса без номенклатуры заказа. Исключите пустую вкладку или заполните её данными");
            return;
        }

        await SetBusy();
        TResponseModel<int> rest = await CommerceRepo.OrderUpdate(CurrentCart);
        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (rest.Response == 0)
        {
            await SetBusy(false);
            return;
        }

        if (rest.Success())
        {
            TResponseModel<OrderDocumentModelDB[]> doc = await CommerceRepo.OrdersRead([rest.Response]);
            CurrentCart.Information = CurrentCart.Information?.Trim();
            CurrentCart = OrderDocumentModelDB.NewEmpty(CurrentUserSession!.UserId);

            await StorageRepo
            .SaveParameter<OrderDocumentModelDB?>(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(CurrentUserSession!.UserId), true);

            NavRepo.NavigateTo($"/issue-card/{doc.Response!.First().HelpdeskId}");
        }
        else
        {
            await SetBusy(false);
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender && CurrentCart is not null)
        {
            await UpdateCachePriceRules();
            CalculateDiscounts();
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReadCurrentUser();
        await OrganizationReset();
        await UpdateCachePriceRules();
        await ActualityData();
        CalculateDiscounts();
    }

    async Task OrganizationReset()
    {
        TPaginationRequestAuthModel<UniversalSelectRequestModel> req = new()
        {
            Payload = new()
            {
                ForUserIdentityId = CurrentUserSession!.IsAdmin ? null : CurrentUserSession!.UserId,
                IncludeExternalData = true,
            },
            SenderActionUserId = CurrentUserSession.UserId,
            PageNum = 0,
            PageSize = int.MaxValue,
            SortBy = nameof(OrderDocumentModelDB.Name),
            SortingDirection = VerticalDirectionsEnum.Up,
        };

        await SetBusy();
        TResponseModel<TPaginationResponseModel<OrganizationModelDB>> res = await CommerceRepo.OrganizationsSelect(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await SetBusy(false);

        if (!res.Success() || res.Response?.Response is null || res.Response.Response.Count == 0)
            return;
        Organizations = res.Response.Response;

        await SetBusy();
        TResponseModel<OrderDocumentModelDB?> current_cart = await StorageRepo
            .ReadParameter<OrderDocumentModelDB>(GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(CurrentUserSession!.UserId));
        await SetBusy(false);
        CurrentCart = current_cart.Response ?? new()
        {
            AuthorIdentityUserId = CurrentUserSession!.UserId,
            Name = "Новый заказ",
        };

        if (CurrentCart.OrganizationId != 0)
        {
            CurrentCart.Organization = Organizations.FirstOrDefault(x => x.Id == CurrentCart.OrganizationId);
            if (CurrentCart.Organization is null)
                CurrentCart.OrganizationId = 0;

            await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(CurrentUserSession!.UserId), true);
        }

        CurrentCart.AddressesTabs ??= [];
        CurrentCart.AddressesTabs.RemoveAll(x => !CurrentOrganization!.Addresses!.Any(y => y.Id == x.AddressOrganizationId));
        if (CurrentCart.AddressesTabs.Count != 0)
        {
            _selectedAddresses = [.. CurrentCart
                .AddressesTabs
                .Select(x => CurrentOrganization!.Addresses!.First(y => y.Id == x.AddressOrganizationId))];
        }
    }
}