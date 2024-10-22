////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Articles;

/// <summary>
/// TagsForArticlesComponent
/// </summary>
public partial class TagsForArticlesComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IHelpdeskRemoteTransmissionService artRepo { get; set; } = default!;


    /// <summary>
    /// Article
    /// </summary>
    [Parameter, EditorRequired]
    public required ArticleModelDB Article { get; set; }


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
        if (string.IsNullOrWhiteSpace(_value))
            return;

        await SetBusy();
        
        TResponseModel<EntryModel[]?> res = await artRepo.TagArticleSet(new() { Name = _value, Id = Article.Id, Set = true });
        IsBusyProgress = false;
        if (!res.Success())
            SnackbarRepo.ShowMessagesResponse(res.Messages);
        else
        {
            Article.Update(res.Response!);
            _value = null;
            if (maRef is not null)
                await maRef.ClearAsync();
        }
        StateHasChanged();
    }

    private async Task OnChipClosed(MudChip<ArticleTagModelDB> chip)
    {
        if (!string.IsNullOrWhiteSpace(chip.Value?.Name))
        {
            await SetBusy();
            
            TResponseModel<EntryModel[]?> res = await artRepo.TagArticleSet(new() { Name = chip.Value.Name, Id = Article.Id, Set = false });
            IsBusyProgress = false;
            if (!res.Success())
                SnackbarRepo.ShowMessagesResponse(res.Messages);
            else
                Article.Update(res.Response!);
        }
    }

    private async Task<IEnumerable<string?>> Search(string value, CancellationToken token)
    {
        await SetBusy();
        
        TResponseModel<string[]?> res = await artRepo.TagsOfArticlesSelect(value);
        IsBusyProgress = false;
        if (!res.Success())
            SnackbarRepo.ShowMessagesResponse(res.Messages);
        List<string> res_data = [.. res.Response?.Where(x => Article.Tags?.Any(y => y.Name.Equals(x, StringComparison.OrdinalIgnoreCase)) != true)];

        if (!string.IsNullOrWhiteSpace(value) && !res_data.Contains(value))
            res_data.Add(value);

        return res_data;
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Article.Tags is not null && Article.Tags.Count != 0)
            Article.Tags.Sort((x, y) => x.Name.CompareTo(y.Name));
    }
}