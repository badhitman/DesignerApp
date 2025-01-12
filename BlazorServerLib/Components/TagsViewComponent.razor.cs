////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components;

/// <summary>
/// TagsViewComponent
/// </summary>
public partial class TagsViewComponent : MetaPropertyBaseComponent
{
    [Inject]
    IStorageTransmission TagsRepo { get; set; } = default!;


    List<TagModelDB> TagsSets { get; set; } = [];

    MudAutocomplete<string?>? maRef;
    string? _value;
    string? TagAdding
    {
        get => _value; set
        {
            _value = value;
            InvokeAsync(AddChip);
        }
    }

    private async Task AddChip()
    {
        if (string.IsNullOrWhiteSpace(_value) || !OwnerPrimaryKey.HasValue)
            return;

        TResponseModel<bool> res = await TagsRepo.TagSet(new()
        {
            PrefixPropertyName = PrefixPropertyName,
            ApplicationName = ApplicationsNames.Single(),
            PropertyName = PropertyName,
            Name = _value,
            Id = OwnerPrimaryKey.Value,
            Set = true
        });

        if (!res.Success())
            SnackbarRepo.ShowMessagesResponse(res.Messages);
        else
        {
            await ReloadTags();
            StateHasChanged();
        }
        _value = "";
        if (maRef is not null)
            await maRef.ClearAsync();
    }

    private async Task OnChipClosed(MudChip<TagModelDB> chip)
    {
        if (!string.IsNullOrWhiteSpace(chip.Value?.TagName) && OwnerPrimaryKey.HasValue)
        {
            await SetBusy();

            await SetBusy();
            TResponseModel<bool> res = await TagsRepo.TagSet(new()
            {
                PrefixPropertyName = PrefixPropertyName,
                ApplicationName = ApplicationsNames.Single(),
                PropertyName = PropertyName,
                Name = chip.Value.TagName,
                Id = OwnerPrimaryKey.Value,
                Set = false
            });
            await ReloadTags();
            await SetBusy(false);
            if (!res.Success())
                SnackbarRepo.ShowMessagesResponse(res.Messages);
        }
    }

    private async Task<IEnumerable<string?>> Search(string value, CancellationToken token)
    {
        TPaginationRequestModel<SelectMetadataRequestModel> req = new()
        {
            Payload = new()
            {
                ApplicationsNames = [],//ApplicationsNames,
                IdentityUsersIds = [], // [CurrentUserSession!.UserId],
                PropertyName = "",//PropertyName,
                OwnerPrimaryKey = 0,//OwnerPrimaryKey,
                PrefixPropertyName = "",//PrefixPropertyName
                SearchQuery = value,
            },
            PageNum = 0,
            PageSize = int.MaxValue,
            SortingDirection = VerticalDirectionsEnum.Down,
        };

        TPaginationResponseModel<TagModelDB> res = await TagsRepo.TagsSelect(req);

        List<string> res_data = [.. res.Response?.Where(x => TagsSets?.Any(y => y.TagName.Equals(x.TagName, StringComparison.OrdinalIgnoreCase)) != true).Select(x => x.TagName)];

        if (!string.IsNullOrWhiteSpace(value) && !res_data.Contains(value))
            res_data.Add(value);

        return res_data.DistinctBy(x => x.ToUpper());
    }

    async Task ReloadTags()
    {
        await SetBusy();
        TPaginationRequestModel<SelectMetadataRequestModel> req = new()
        {
            Payload = new()
            {
                ApplicationsNames = this.ApplicationsNames,
                IdentityUsersIds = [],
                PropertyName = PropertyName,
                OwnerPrimaryKey = OwnerPrimaryKey,
                PrefixPropertyName = PrefixPropertyName,
            },
            PageNum = 0,
            PageSize = int.MaxValue,
            SortingDirection = VerticalDirectionsEnum.Down,
        };

        TPaginationResponseModel<TagModelDB> res = await TagsRepo.TagsSelect(req);

        await SetBusy(false);
        if (res.Response is not null)
            TagsSets = res.Response;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await SetBusy();
        await ReadCurrentUser();
        await ReloadTags();
    }
}