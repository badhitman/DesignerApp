////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Users;

/// <summary>
/// ClaimRowComponent
/// </summary>
public partial class ClaimRowComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IIdentityTransmission IdentityRepo { get; set; } = default!;


    /// <summary>
    /// Claim
    /// </summary>
    [Parameter, EditorRequired]
    public required ClaimBaseModel Claim { get; set; }

    /// <summary>
    /// ClaimArea
    /// </summary>
    [Parameter, EditorRequired]
    public ClaimAreasEnum ClaimArea { get; set; }

    /// <summary>
    /// OwnerId
    /// </summary>
    [Parameter, EditorRequired]
    public required string OwnerId { get; set; }

    /// <summary>
    /// ReloadHandler
    /// </summary>
    [Parameter, EditorRequired]
    public required Action ReloadHandler { get; set; }

    bool isEdit;
    string? claimType;
    string? claimValue;

    bool CantSave => string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimValue) || Claim.Equals(claimType, claimValue);

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        CancelEdit();
    }

    void CancelEdit()
    {
        claimType = Claim.ClaimType;
        claimValue = Claim.ClaimValue;
        isEdit = false;
    }

    async Task SaveClaim()
    {
        await SetBusy();
        ResponseBaseModel res = await IdentityRepo.ClaimUpdateOrCreate(new() { ClaimArea = ClaimArea, ClaimUpdate = ClaimModel.Build(Claim.Id, claimType, claimValue, OwnerId) });
        IsBusyProgress = false;
        if (!res.Success())
            throw new Exception(res.Message());

        isEdit = false;
        ReloadHandler();
    }

    bool initRemoveClaim = false;
    async Task RemoveClaim()
    {
        if (!initRemoveClaim)
        {
            initRemoveClaim = true;
            return;
        }

        await SetBusy();
        ResponseBaseModel res = await IdentityRepo.ClaimDelete(new() { ClaimArea = ClaimArea, Id = Claim.Id });
        IsBusyProgress = false;
        initRemoveClaim = false;
        if (!res.Success())
            throw new Exception(res.Message());

        ReloadHandler();
    }
}