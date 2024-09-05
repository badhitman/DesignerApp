////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using BlazorWebLib.Components.Helpdesk;

namespace BlazorWebLib.Components.Commerce.Pages;

/// <summary>
/// AddressForOrganizationPage
/// </summary>
public partial class AddressForOrganizationPage : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// AddressForOrganization
    /// </summary>
    [Parameter]
    public int AddressForOrganization { get; set; } = default!;


    AddressOrganizationModelDB AddressCurrent { get; set; } = default!;
    AddressOrganizationModelDB AddressEdit { get; set; } = default!;

    bool CanSave =>
        (AddressEdit.Address != AddressCurrent.Address ||
        AddressEdit.Contacts != AddressCurrent.Contacts ||
        AddressEdit.Name != AddressCurrent.Name ||
        AddressEdit.ParentId != AddressCurrent.ParentId) &&
        SelectedRubric is not null;

    RubricIssueHelpdeskLowModel? SelectedRubric;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<AddressOrganizationModelDB[]?> res_address = await CommerceRepo
            .AddressesOrganizationsRead([AddressForOrganization]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res_address.Messages);
        AddressCurrent = res_address.Response!.Single();
        AddressEdit = GlobalTools.CreateDeepCopy(AddressCurrent) ?? throw new Exception();
        IsBusyProgress = true;
        TResponseModel<List<RubricIssueHelpdeskModelDB>?> res_rubric = await HelpdeskRepo.RubricRead(AddressCurrent.ParentId);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res_rubric.Messages);
        if (res_rubric.Success() && res_rubric.Response is not null && res_rubric.Response.Count != 0)
        {
            RubricIssueHelpdeskModelDB r = res_rubric.Response.First();
            SelectedRubric = new RubricIssueHelpdeskLowModel()
            {
                Name = r.Name,
                Description = r.Description,
                Id = r.Id,
                IsDisabled = r.IsDisabled,
                ParentRubricId = r.ParentRubricId,
                ProjectId = r.ProjectId,
                SortIndex = r.SortIndex,
            };
        }
    }

    RubricSelectorComponent rubricSelector_ref = default!;

    void ResetEdit()
    {
        AddressEdit = GlobalTools.CreateDeepCopy(AddressCurrent) ?? throw new Exception();
        if (rubricSelector_ref.SelectedRubricId != AddressEdit.ParentId)
        {
            rubricSelector_ref.SelectedRubricId = AddressEdit.ParentId;
            rubricSelector_ref.StateHasChangedCall();
        }
    }

    async Task SaveAddress()
    {
        if (!CanSave)
            return;

        IsBusyProgress = true;
        TResponseModel<int?> res = await CommerceRepo.AddressOrganizationUpdate(new AddressOrganizationBaseModel()
        {
            Address = AddressEdit.Address!,
            Name = AddressEdit.Name!,
            ParentId = SelectedRubric!.Id,
            Contacts = AddressEdit.Contacts,
            Id = AddressForOrganization,
        });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (!res.Success() || res.Response is null)
            return;

        AddressCurrent = GlobalTools.CreateDeepCopy(AddressEdit) ?? throw new Exception();
    }

    void RubricSelectAction(RubricIssueHelpdeskLowModel? selectedRubric)
    {
        SelectedRubric = selectedRubric;
        AddressEdit.ParentId = selectedRubric?.Id ?? 0;
        StateHasChanged();
    }
}