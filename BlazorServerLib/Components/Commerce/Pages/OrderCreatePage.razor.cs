////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Pages;

/// <summary>
/// OrderCreatePage
/// </summary>
public partial class OrderCreatePage : BlazorBusyComponentBaseModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StoreRepo { get; set; } = default!;

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

    UserInfoMainModel user = default!;
    OrderDocumentModelDB CurrentCart = default!;
    readonly Func<AddressOrganizationModelDB, string> converter = p => p.Name;

    bool _expanded;

    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }

    IEnumerable<AddressOrganizationModelDB>? _prevSelectedAddresses;
    List<AddressOrganizationModelDB>? _selectedAddresses = [];
    IEnumerable<AddressOrganizationModelDB>? SelectedAddresses
    {
        get => _selectedAddresses ?? [];
        set
        {
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
            _prevSelectedAddresses = _qr
                .Where(x => x.Rows is null || x.Rows.Count == 0)
                .Select(Convert)
                .ToArray();
            if (_prevSelectedAddresses.Any())
            {
                CurrentCart.AddressesTabs!.RemoveAll(x => _prevSelectedAddresses!.Any(y => y.Id == x.AddressOrganizationId));
                _selectedAddresses = [.. CurrentCart.AddressesTabs.Select(Convert)];
                InvokeAsync(async () => { await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId)); });
            }

            // адреса/вкладки, которые имеют строки: требуют подтверждения у пользователя
            _prevSelectedAddresses = _qr
                .Where(x => x.Rows is not null && x.Rows.Count != 0)
                .Select(Convert)
                .ToArray();

            if (_prevSelectedAddresses.Any())
                _visibleChangeAddresses = true;
            else
                _prevSelectedAddresses = null;
        }
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

    void SubmitChangeAddresses()
    {
        CurrentCart.AddressesTabs ??= [];
        if (_prevSelectedAddresses is null)
            CurrentCart.AddressesTabs.Clear();
        else
            CurrentCart.AddressesTabs!.RemoveAll(x => !_prevSelectedAddresses.Any(y => y.Id == x.AddressOrganizationId));

        _selectedAddresses = [.. _prevSelectedAddresses];
        _selectedAddresses.Clear();
        _prevSelectedAddresses = null;
        _visibleChangeAddresses = false;
        InvokeAsync(async () => { await StorageRepo.SaveParameter(CurrentCart, GlobalStaticConstants.CloudStorageMetadata.OrderCartForUser(user.UserId)); });
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
        TResponseModel<TPaginationResponseModel<OrganizationModelDB>> res = await CommerceRepo.OrganizationsSelect(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        IsBusyProgress = false;

        if (!res.Success() || res.Response?.Response is null || res.Response.Response.Count == 0)
            return;
        Organizations = res.Response.Response;

        IsBusyProgress = true;
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
    }
}