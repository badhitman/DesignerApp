////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OrganizationEditComponent
/// </summary>
public partial class OrganizationEditComponent : BlazorBusyComponentBaseAuthModel
{
    [Inject]
    NavigationManager NavigationRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// OrganizationId
    /// </summary>
    [Parameter]
    public int? OrganizationId { get; set; }


    OrganizationModelDB? currentOrg;
    OrganizationModelDB? editOrg;

    async Task CancelEditRequestOrganization()
    {
        if (editOrg is null || editOrg.Equals(currentOrg))
            return;

        editOrg = GlobalTools.CreateDeepCopy(currentOrg);

        TAuthRequestModel<OrganizationModelDB> req = new() { Payload = editOrg!, SenderActionUserId = CurrentUserSession!.UserId };
        await SetBusy();
        
        TResponseModel<int> res = await CommerceRepo.OrganizationUpdate(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await ReadOrganization();
    }

    async Task ConfirmChangeOrganization()
    {
        if (editOrg is null)
            throw new ArgumentNullException(nameof(editOrg));

        OrganizationModelDB req = GlobalTools.CreateDeepCopy(editOrg)!;

        if (!string.IsNullOrWhiteSpace(editOrg.NewBankBIC))
            req.BankBIC = editOrg.NewBankBIC;

        if (!string.IsNullOrWhiteSpace(editOrg.NewBankName))
            req.BankName = editOrg.NewBankName;

        if (!string.IsNullOrWhiteSpace(editOrg.NewINN))
            req.INN = editOrg.NewINN;

        if (!string.IsNullOrWhiteSpace(editOrg.NewOGRN))
            req.OGRN = editOrg.NewOGRN;

        if (!string.IsNullOrWhiteSpace(editOrg.NewCorrespondentAccount))
            req.CorrespondentAccount = editOrg.NewCorrespondentAccount;

        if (!string.IsNullOrWhiteSpace(editOrg.NewLegalAddress))
            req.LegalAddress = editOrg.NewLegalAddress;

        if (!string.IsNullOrWhiteSpace(editOrg.NewCurrentAccount))
            req.CurrentAccount = editOrg.NewCurrentAccount;

        if (!string.IsNullOrWhiteSpace(editOrg.NewKPP))
            req.KPP = editOrg.NewKPP;

        if (!string.IsNullOrWhiteSpace(editOrg.NewName))
            req.Name = editOrg.NewName;

        await SetBusy();
        
        TResponseModel<bool> res = await CommerceRepo.OrganizationSetLegal(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        NavigationRepo.ReloadPage();
    }

    async Task SaveOrganization()
    {
        if (editOrg is null || editOrg.Equals(currentOrg))
            throw new ArgumentNullException(nameof(editOrg));

        TAuthRequestModel<OrganizationModelDB> req = new() { Payload = editOrg!, SenderActionUserId = CurrentUserSession!.UserId };
        await SetBusy();
        
        TResponseModel<int> res = await CommerceRepo.OrganizationUpdate(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (OrganizationId == 0)
        {
            NavigationRepo.NavigateTo($"/organization-edit/{res.Response}");
            return;
        }

        await ReadOrganization();
    }

    async Task ReadOrganization()
    {
        if (OrganizationId is null)
            return;

        await SetBusy();
        
        TResponseModel<OrganizationModelDB[]> res = await CommerceRepo.OrganizationsRead([OrganizationId.Value]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        currentOrg = res.Response?.FirstOrDefault();
        if (currentOrg is not null && (currentOrg.Users?.Any(x => x.UserPersonIdentityId == CurrentUserSession!.UserId) != true && !CurrentUserSession!.IsAdmin && CurrentUserSession!.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Contains(x)) != true))
        {
            currentOrg = null;
            return;
        }
        editOrg = currentOrg is null
            ? null
            : GlobalTools.CreateDeepCopy(currentOrg);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        OrganizationId ??= 0;
        await ReadCurrentUser();

        if (OrganizationId == 0)
        {
            currentOrg = new()
            {
                BankBIC = string.Empty,
                BankName = string.Empty,
                CorrespondentAccount = string.Empty,
                CurrentAccount = string.Empty,
                Email = string.Empty,
                INN = string.Empty,
                KPP = string.Empty,
                LegalAddress = string.Empty,
                Name = string.Empty,
                OGRN = string.Empty,
                Phone = string.Empty,
            };
            editOrg = GlobalTools.CreateDeepCopy(currentOrg);
            return;
        }

        await ReadOrganization();
    }
}