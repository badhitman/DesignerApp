////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using MudBlazor;
using SharedLib;
using BlazorLib;

namespace BlazorWebLib.Components.Users;

/// <summary>
/// ClaimsManageComponent
/// </summary>
public partial class ClaimsManageComponent
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IIdentityTransmission IdentityRepo { get; set; } = default!;


    /// <summary>
    /// ClaimArea
    /// </summary>
    [Parameter, EditorRequired]
    public required ClaimAreasEnum ClaimArea { get; set; }

    /// <summary>
    /// OwnerId
    /// </summary>
    [Parameter, EditorRequired]
    public required string OwnerId { get; set; }


    List<ClaimBaseModel>? claims;
    IEnumerable<ResultMessage>? Messages;
    string? claimType;
    string? claimValue;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            claims = await IdentityRepo.GetClaims(new() { ClaimArea = ClaimArea, OwnerId = OwnerId });//
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    async Task AddClaim()
    {
        if (string.IsNullOrWhiteSpace(claimValue) || string.IsNullOrWhiteSpace(claimType))
        {
            SnackbarRepo.Error("Укажите имя и значение");
            return;
        }

        claimType = claimType.Trim();
        claimValue = claimValue.Trim();

        if (claimType == GlobalStaticConstants.TelegramIdClaimName || claimType == ClaimTypes.GivenName || claimType == ClaimTypes.Surname)
        {
            SnackbarRepo.Error("Имя зарезервировано. Укажите другое");
            return;
        }

        ResponseBaseModel res = await IdentityRepo.ClaimUpdateOrCreate(new() { ClaimArea = ClaimArea, ClaimUpdate = new() { ClaimType = claimType?.Trim(), ClaimValue = claimValue?.Trim(), OwnerId = OwnerId } });

        if (!res.Success())
        {
            Messages = res.Messages;
            return;
        }

        claimType = null;
        claimValue = null;

        claims = await IdentityRepo.GetClaims(new() { ClaimArea = ClaimArea, OwnerId = OwnerId });
    }
}