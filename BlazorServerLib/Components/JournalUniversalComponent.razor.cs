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



    private string? searchString;

    /// <summary>
    /// ColumnsNames
    /// </summary>
    EntryAltModel[]? ColumnsNames { get; set; } = [];

    private int totalItems;

    private MudTable<KeyValuePair<int, Dictionary<string, object>>> table = default!;


    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<KeyValuePair<int, Dictionary<string, object>>>> ServerReload(TableState state, CancellationToken token)
    {
        if (DocumentType is null)
            throw new Exception();

        IsBusyProgress = true;
        TPaginationResponseModel<KeyValuePair<int, Dictionary<string, object>>> res = await JournalRepo
            .SelectJournalPart(new SelectJournalPartRequestModel() { DocumentNameOrId = DocumentType, SortBy = state.SortLabel, PageNum = state.Page, PageSize = state.PageSize, SortingDirection = state.SortDirection == SortDirection.Descending ? VerticalDirectionsEnum.Down : VerticalDirectionsEnum.Up }, ProjectId);
        IsBusyProgress = false;

        totalItems = res.TotalRowsCount;
        return new TableData<KeyValuePair<int, Dictionary<string, object>>>() { TotalItems = totalItems, Items = res.Response };
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
            throw new Exception();

        IsBusyProgress = true;
        TResponseModel<EntryAltModel[]?> res_columns = await JournalRepo.GetColumnsForJournal(DocumentType, ProjectId);
        ColumnsNames = res_columns.Response;
        await table.ReloadServerData();
        IsBusyProgress = false;
    }
}