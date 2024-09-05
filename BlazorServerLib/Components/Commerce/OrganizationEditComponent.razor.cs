////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// OrganizationEditComponent
/// </summary>
public partial class OrganizationEditComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    NavigationManager NavigationRepo { get; set; } = default!;

    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider AuthRepo { get; set; } = default!;


    /// <summary>
    /// OrganizationId
    /// </summary>
    [Parameter]
    public int? OrganizationId { get; set; }


    UserInfoMainModel user = default!;
    OrganizationModelDB? currentOrg;
    OrganizationModelDB? editOrg;

    async Task CancelEditRequestOrganization()
    {
        if (editOrg is null || editOrg.Equals(currentOrg))
            return;

        editOrg = GlobalTools.CreateDeepCopy(currentOrg);

        TAuthRequestModel<OrganizationModelDB> req = new() { Payload = editOrg!, SenderActionUserId = user.UserId };
        IsBusyProgress = true;
        TResponseModel<int?> res = await CommerceRepo.OrganizationUpdate(req);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await ReadOrganization();
    }

    async Task SaveOrganization()
    {
        if (editOrg is null || !editOrg.Equals(currentOrg))
            throw new ArgumentNullException(nameof(editOrg));

        TAuthRequestModel<OrganizationModelDB> req = new() { Payload = editOrg!, SenderActionUserId = user.UserId };
        IsBusyProgress = true;
        TResponseModel<int?> res = await CommerceRepo.OrganizationUpdate(req);
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

        IsBusyProgress = true;
        TResponseModel<OrganizationModelDB[]?> res = await CommerceRepo.OrganizationsRead([OrganizationId.Value]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        currentOrg = res.Response?.FirstOrDefault();
        if (currentOrg is not null && (currentOrg.Users?.Any(x => x.UserPersonIdentityId == user.UserId) != true && !user.IsAdmin && user.Roles?.Any(x => GlobalStaticConstants.Roles.AllHelpDeskRoles.Contains(x)) != true))
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

        AuthenticationState state = await AuthRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();
        await ReadOrganization();
    }
}