////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// AddressesForOrganizationComponent
/// </summary>
public partial class AddressesForOrganizationComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// Organization
    /// </summary>
    [Parameter, EditorRequired]
    public required OrganizationModelDB Organization { get; set; }


    RubricIssueHelpdeskLowModel? SelectedRubric;

    Dictionary<int, List<RubricIssueHelpdeskModelDB>> RubriciesCached = [];
    string? addingAddress, addingContacts, addingName;

    bool CanCreate =>
        !string.IsNullOrWhiteSpace(addingAddress) &&
        !string.IsNullOrWhiteSpace(addingContacts) &&
        !string.IsNullOrWhiteSpace(addingName) &&
        SelectedRubric is not null;

    bool _expanded;
    private void OnExpandCollapseClick()
    {
        _expanded = !_expanded;
    }

    async Task AddAddress()
    {
        if (!CanCreate)
            return;

        IsBusyProgress = true;
        TResponseModel<int> res = await CommerceRepo.AddressOrganizationUpdate(new AddressOrganizationBaseModel()
        {
            Address = addingAddress!,
            Name = addingName!,
            ParentId = SelectedRubric!.Id,
            OrganizationId = Organization.Id,
            Contacts = addingContacts,
        });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (!res.Success())
            return;

        Organization.Addresses ??= [];
        Organization.Addresses.Add(new()
        {
            Address = addingAddress!,
            Name = addingName!,
            ParentId = SelectedRubric!.Id,
            OrganizationId = Organization.Id,
            Contacts = addingContacts,
            Id = res.Response,
        });

        ToggleMode();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await UpdateCacheRubrics();
    }

    string? _last_request;
    async Task UpdateCacheRubrics()
    {
        Organization.Addresses ??= [];
        int[] added_rubrics = [..Organization
            .Addresses
            .Where(x => !RubriciesCached.ContainsKey(x.ParentId))
            .Select(x => x.ParentId).Distinct()];

        if (added_rubrics.Length != 0)
        {
            string _curr_request = string.Join(",", added_rubrics);
            if (_curr_request == _last_request)
                return;
            _last_request = _curr_request;

            IsBusyProgress = true;
            foreach (int i in added_rubrics)
            {
                TResponseModel<List<RubricIssueHelpdeskModelDB>?> res = await HelpdeskRepo.RubricRead(i);
                if (res.Success() && res.Response is not null)
                    RubriciesCached.Add(i, res.Response);
            }
            IsBusyProgress = false;
            StateHasChanged();
        }
    }

    string? GetCity(AddressOrganizationModelDB ad)
    {
        if (!RubriciesCached.TryGetValue(ad.ParentId, out List<RubricIssueHelpdeskModelDB>? value))
            return null;

        return value.LastOrDefault()?.Name;
    }

    void RubricSelectAction(RubricIssueHelpdeskLowModel? selectedRubric)
    {
        SelectedRubric = selectedRubric;
        StateHasChanged();
    }

    void ToggleMode()
    {
        _expanded = !_expanded;
        addingAddress = null;
        addingContacts = null;
        addingName = null;
    }


}