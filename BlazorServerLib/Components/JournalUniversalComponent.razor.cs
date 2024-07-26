////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components;

/// <summary>
/// Журнал документов (универсальный)
/// </summary>
public partial class JournalUniversalComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IJournalUniversalService JournalRepo { get; set; } = default!;


    /// <summary>
    /// Тип документа
    /// </summary>
    [Parameter]
    public string? DocumentType { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    [Parameter, SupplyParameterFromQuery]
    public int? ProjectId { get; set; }


    List<KeyValuePair<int, Dictionary<string, object>>> documents = default!;

    private string? searchString;

    /// <summary>
    /// ColumnsNames
    /// </summary>
    EntryAltModel[]? ColumnsNames { get; set; } = [];

    private int totalItems;

    private IEnumerable<KeyValuePair<int, Dictionary<string, object>>> pagedData = default!;
    private MudTable<KeyValuePair<int, Dictionary<string, object>>> table = default!;


    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<KeyValuePair<int, Dictionary<string, object>>>> ServerReload(TableState state, CancellationToken token)
    {
        if(DocumentType is null)
            throw new ArgumentNullException(nameof(DocumentType));

        IEnumerable<KeyValuePair<int, Dictionary<string, object>>> data;

        IsBusyProgress = true;
        TPaginationResponseModel<KeyValuePair<int, Dictionary<string, object>>[]?> res = await JournalRepo
            .SelectJournalPart(new TPaginationRequestModel<string>() { Request = DocumentType, SortBy = state.SortLabel }, ProjectId);
        IsBusyProgress = false;

#if DEBUG
        data = documents;
        if (!string.IsNullOrWhiteSpace(searchString))
            data = data
                .Where(element => element.Value.Any(z => z.Value.ToString()?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true))
                .ToArray();
#endif

        totalItems = data.Count();
        if (!string.IsNullOrWhiteSpace(state.SortLabel))
            if (state.SortDirection == SortDirection.Ascending)
                data = data.OrderBy(x => x.Value[state.SortLabel]);
            else
                data = data.OrderByDescending(x => x.Value[state.SortLabel]);

        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        return new TableData<KeyValuePair<int, Dictionary<string, object>>>() { TotalItems = totalItems, Items = pagedData };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }

    /// <inheritdoc/>
    protected override async void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(DocumentType))
            return;

        IsBusyProgress = true;
        TResponseModel<EntryAltModel[]?> res = await JournalRepo.GetColumnsForJournal(DocumentType, ProjectId);
        ColumnsNames = res.Response;
        IsBusyProgress = false;

        

        documents =
        [
            new(1, new()
            {
                { "Name", "Sam" }, { "Position", "CPA" }, { "YearsEmployed", 23 }, { "Salary", 87_000 }, { "Rating", 4 }
            }),
            new(2, new()
            {
                { "Name", "Alicia" }, { "Position", "Product Manager" }, { "YearsEmployed", 11 }, { "Salary", 143_000 }, { "Rating", 5 }
            }),
            new(3, new()
            {
                { "Name", "Ira" }, { "Position", "Developer" }, { "YearsEmployed", 4 }, { "Salary", 92_000 }, { "Rating", 3 }
            }),
            new(4, new()
            {
                { "Name", "John" }, { "Position", "IT Director" }, { "YearsEmployed", 17 }, { "Salary", 229_000 }, { "Rating", 4 }
            })
        ];

    }
}