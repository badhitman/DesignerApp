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
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService artRepo { get; set; } = default!;


    /// <summary>
    /// Article
    /// </summary>
    [Parameter, EditorRequired]
    public required ArticleModelDB Article { get; set; }


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

        IsBusyProgress = true;
        TResponseModel<EntryModel[]?> res = await artRepo.TagArticleSet(new() { Name = _value, Id = Article.Id, Set = true });
        IsBusyProgress = false;
        if (!res.Success())
            SnackbarRepo.ShowMessagesResponse(res.Messages);
        else
        {
            Article.Update(res.Response!);
            _value = null;
        }
        StateHasChanged();
    }

    private async Task OnChipClosed(MudChip<ArticleTagModelDB> chip)
    {
        if (!string.IsNullOrWhiteSpace(chip.Value?.Name))
        {
            IsBusyProgress = true;
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
        IsBusyProgress = true;
        TResponseModel<string[]?> res = await artRepo.TagsOfArticlesSelect(value);
        IsBusyProgress = false;
        if (!res.Success())
            SnackbarRepo.ShowMessagesResponse(res.Messages);
        List<string> res_data = [.. res.Response];

        if (!string.IsNullOrWhiteSpace(value) && !res_data.Contains(value))
            res_data.Add(value);

        return res_data;
    }
}