////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OrderCreateComponent
/// </summary>
public partial class OrderCreateComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StorageRepo { get; set; } = default!;

    [Inject]
    NavigationManager NavRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider AuthRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    bool _visibleChangeAddresses;
    bool _visibleChangeOrganization;
    readonly DialogOptions _dialogOptions = new() { FullWidth = true };

    UserInfoMainModel user = default!;
    OrderDocumentModelDB CurrentCart = default!;
    readonly Func<AddressOrganizationModelDB, string> converter = p => p.Name;

    List<OrganizationModelDB> Organizations { get; set; } = [];
    OrganizationModelDB? prevCurrOrg;
    OrganizationModelDB? CurrentOrganization
    {
        get => CurrentCart.Organization;
        set
        {
            if (SelectedAddresses?.Any() == true)
            {
                _visibleChangeOrganization = true;
                prevCurrOrg = value;
                return;
            }

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
            if (_prevSelectedAddresses is not null)
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
                InvokeAsync(async () => await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId)));
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
                InvokeAsync(async () => { await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId)); });
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
    Dictionary<int, PriceRuleForOfferModelDB[]?> RulesCache = [];
    readonly Dictionary<int, decimal> DiscountsDetected = [];

    async Task UpdateCachePriceRules()
    {
        if (CurrentCart.AddressesTabs is null)
        {
            AllRows = [];
            return;
        }

        AllRows = [.. CurrentCart.AddressesTabs?.Where(x => x.Rows is not null).SelectMany(x => x.Rows!)];
        GroupingRows = AllRows.GroupBy(x => x.OfferId).ToList();
        List<int> offers_load = [.. GroupingRows.Where(dc => !RulesCache.ContainsKey(dc.Key)).Select(x => x.Key).Distinct()];

        if (offers_load.Count == 0)
            return;

        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<PriceRuleForOfferModelDB[]> res = await CommerceRepo.PricesRulesGetForOffers([.. offers_load]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        offers_load.ForEach(x => RulesCache.Add(x, res.Response?.Where(y => x == y.OfferId && !y.IsDisabled).ToArray()));

        StateHasChanged();
    }

    void CalculateDiscounts()
    {
        if (GroupingRows.Any(x => x.Any(y => y.Offer is null)))
            return;

        string json_dump_discounts_before = JsonConvert.SerializeObject(DiscountsDetected);
        DiscountsDetected.Clear();
        foreach (IGrouping<int, RowOfOrderDocumentModelDB> node in GroupingRows)
        {
            int qnt = node.Sum(y => y.Quantity); // всего количество в заказе
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
        CurrentCart.AddressesTabs ??= [];
        if (_prevSelectedAddresses is null || !_prevSelectedAddresses.Any())
            CurrentCart.AddressesTabs.Clear();
        else
            CurrentCart.AddressesTabs.RemoveAll(x => !_prevSelectedAddresses.Any(y => y.Id == x.AddressOrganizationId));

        _selectedAddresses = [.. _prevSelectedAddresses];
        _prevSelectedAddresses = null;
        _visibleChangeAddresses = false;
        IsBusyProgress = true;
        await Task.Delay(1);
        await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId));
        IsBusyProgress = false;
    }

    void CancelChangeAddresses()
    {
        _visibleChangeAddresses = false;
        _prevSelectedAddresses = null;
    }

    void SubmitChangeOrganizations()
    {
        CurrentCart.Organization = prevCurrOrg;
        CurrentCart.OrganizationId = prevCurrOrg?.Id ?? 0;
        prevCurrOrg = null;
        CurrentCart.AddressesTabs?.RemoveAll(x => !CurrentOrganization!.Addresses!.Any(y => y.Id == x.AddressOrganizationId));
        ResetAddresses();
        _visibleChangeOrganization = false;
    }

    async void DocumentUpdateAction()
    {
        CurrentCart.AddressesTabs?.RemoveAll(x => !CurrentOrganization!.Addresses!.Any(y => y.Id == x.AddressOrganizationId));
        await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId));
        StateHasChanged();
    }

    void CancelChangeOrganizations()
    {
        _visibleChangeOrganization = false;
        prevCurrOrg = null;
    }

    void ResetAddresses()
    {
        _selectedAddresses?.Clear();
        InvokeAsync(async () => { await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId)); });
    }

    async Task OrderDocumentSendAsync()
    {
        if (CurrentCart.AddressesTabs?.Any(x => x.Rows is null || x.Rows.Count == 0) == true)
        {
            SnackbarRepo.Error("Присутствуют адреса без номенклатуры заказа. Исключите пустую вкладку или заполните её данными");
            return;
        }

        CurrentCart.PrepareForSave();
        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<int> rest = await CommerceRepo.OrderUpdate(CurrentCart);

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Success())
        {
            TResponseModel<OrderDocumentModelDB[]> doc = await CommerceRepo.OrdersRead([rest.Response]);
            CurrentCart.Information = CurrentCart.Information?.Trim();
            CurrentCart = new()
            {
                AuthorIdentityUserId = user.UserId,
                Name = "Новый заказ",
            };
            await StorageRepo
            .SaveParameter<OrderDocumentModelDB?>(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId));

            NavRepo.NavigateTo($"/issue-card/{doc.Response!.First().HelpdeskId}");
        }
        else
        {
            IsBusyProgress = false;
            StateHasChanged();
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            await UpdateCachePriceRules();
            CalculateDiscounts();
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await AuthRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();

        TPaginationRequestModel<OrganizationsSelectRequestModel> req = new()
        {
            Payload = new()
            {
                ForUserIdentityId = user.IsAdmin ? null : user.UserId,
                IncludeExternalData = true,
            },
            PageNum = 0,
            PageSize = int.MaxValue,
            SortBy = nameof(OrderDocumentModelDB.Name),
            SortingDirection = VerticalDirectionsEnum.Up,
        };

        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<TPaginationResponseModel<OrganizationModelDB>> res = await CommerceRepo.OrganizationsSelect(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        IsBusyProgress = false;

        if (!res.Success() || res.Response?.Response is null || res.Response.Response.Count == 0)
            return;
        Organizations = res.Response.Response;

        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<OrderDocumentModelDB?> current_cart = await StorageRepo
            .ReadParameter<OrderDocumentModelDB>(GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId));
        IsBusyProgress = false;
        CurrentCart = current_cart.Response ?? new()
        {
            AuthorIdentityUserId = user.UserId,
            Name = "Новый заказ",
        };

        if (CurrentCart.OrganizationId != 0)
        {
            CurrentCart.Organization = Organizations.FirstOrDefault(x => x.Id == CurrentCart.OrganizationId);
            if (CurrentCart.Organization is null)
                CurrentCart.OrganizationId = 0;

            await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId));
        }

        CurrentCart.AddressesTabs ??= [];
        CurrentCart.AddressesTabs.RemoveAll(x => !CurrentOrganization!.Addresses!.Any(y => y.Id == x.AddressOrganizationId));
        if (CurrentCart.AddressesTabs.Count != 0)
        {
            _selectedAddresses = [.. CurrentCart
                .AddressesTabs
                .Select(x => CurrentOrganization!.Addresses!.First(y => y.Id == x.AddressOrganizationId))];
        }

        await UpdateCachePriceRules();
        CalculateDiscounts();
    }
}