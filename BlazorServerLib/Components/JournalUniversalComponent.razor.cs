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

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    NavigationManager NavigationRepo { get; set; } = default!;

    /// <summary>
    /// Тип документа
    /// </summary>
    [Parameter, EditorRequired]
    public required string DocumentNameOrIdType { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    [Parameter]
    public int? ProjectId { get; set; }

    /// <summary>
    /// Отображать навигацию между журналами
    /// </summary>
    [Parameter]
    public bool ShowNavigation { get; set; }

    string? SelectedJournal
    {
        get => DocumentNameOrIdType;
        set
        {
            if (value != DocumentNameOrIdType)
                NavigationRepo.NavigateTo($"/documents-journal/{value}");
        }
    }

    DocumentSchemeConstructorModelDB[]? DocumentsSchemes { get; set; }

    private string? searchString;

    /// <summary>
    /// ColumnsNames
    /// </summary>
    EntryAltModel[]? ColumnsNames { get; set; }

    private int totalItems;

    private MudTable<KeyValuePair<int, Dictionary<string, object>>>? table;

    EntryAltTagModel[]? MySchemas = null;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<KeyValuePair<int, Dictionary<string, object>>>> ServerReload(TableState state, CancellationToken token)
    {
        if (DocumentNameOrIdType is null)
            throw new Exception();

        IsBusyProgress = true;
        TPaginationResponseModel<KeyValuePair<int, Dictionary<string, object>>> res = await JournalRepo
            .SelectJournalPart(new SelectJournalPartRequestModel()
            {
                SearchString = searchString,
                DocumentNameOrId = DocumentNameOrIdType,
                SortBy = state.SortLabel,
                PageNum = state.Page,
                PageSize = state.PageSize,
                SortingDirection = state.SortDirection == SortDirection.Descending ? VerticalDirectionsEnum.Down : VerticalDirectionsEnum.Up
            }, ProjectId);
        IsBusyProgress = false;

        totalItems = res.TotalRowsCount;
        return new TableData<KeyValuePair<int, Dictionary<string, object>>>() { TotalItems = totalItems, Items = res.Response };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table?.ReloadServerData();
    }

    /// <inheritdoc/>
    protected override async void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(DocumentNameOrIdType) && !ShowNavigation)
            throw new Exception();

        if (ShowNavigation)
        {
            IsBusyProgress = true;
            MySchemas = await JournalRepo.GetMyDocumentsSchemas();
            IsBusyProgress = false;

            if (MySchemas.Length != 0 && !MySchemas.Any(x => x.Id == DocumentNameOrIdType))
            {
                SelectedJournal = MySchemas[0].Id;
                return;
            }
        }
        IsBusyProgress = true;
        TResponseModel<DocumentSchemeConstructorModelDB[]?> res_fs = await JournalRepo.FindDocumentSchemes(DocumentNameOrIdType, ProjectId);
        DocumentsSchemes = res_fs.Response;
        IsBusyProgress = false;

        if (SelectedJournal is null)
            ColumnsNames = null;
        else
        {
            IsBusyProgress = true;
            TResponseModel<EntryAltModel[]?> res = await JournalRepo.GetColumnsForJournal(SelectedJournal, ProjectId);
            ColumnsNames = res.Response;
            SnackbarRepo.ShowMessagesResponse(res.Messages);
            IsBusyProgress = false;
        }
        if (table is not null)
            await table.ReloadServerData();
    }
}