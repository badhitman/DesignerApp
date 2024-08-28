////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Documents.Forms;

/// <summary>
/// TableFormOfTabConstructorComponent
/// </summary>
public partial class TableFormOfTabConstructorComponent : FormBaseCore
{
    /// <inheritdoc/>
    [CascadingParameter]
    public List<ValueDataForSessionOfDocumentModelDB>? SessionValues { get; set; }

    /// <summary>
    /// Вкладка/таб
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormToTabJoinConstructorModelDB Join { get; set; }

    KeyValuePair<uint, ValueDataForSessionOfDocumentModelDB[]>[]? RowsData => SessionValues?
        .Where(x => x.RowNum > 0)
        .GroupBy(x => x.RowNum)
        .Select(x => new KeyValuePair<uint, ValueDataForSessionOfDocumentModelDB[]>(x.Key, [.. x]))
        .ToArray();

    /// <summary>
    /// Таблица редактируется через свои собственные посадочные стрраницы формы
    /// </summary>
    public override bool IsEdited => false;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server, with a token for canceling this request
    /// </summary>
    private Task<TableData<KeyValuePair<uint, Dictionary<string, object>>>> ServerReload(TableState state, CancellationToken token)
    {
        KeyValuePair<uint, ValueDataForSessionOfDocumentModelDB[]>[]? rows = RowsData;
        if (rows is null || rows.Length == 0)
            return Task.FromResult(new TableData<KeyValuePair<uint, Dictionary<string, object>>>() { TotalItems = 0, Items = [] });

        // Forward the provided token to methods which support it
        //var data = await httpClient.GetFromJsonAsync<List<Dictionary<uint, Dictionary<string,object>>>>("webapi/periodictable", token);
        // Simulate a long-running operation
        //await Task.Delay(300, token);
        // Get the total count

        // Get the paged data
        List<KeyValuePair<uint, ValueDataForSessionOfDocumentModelDB[]>> pagedData = rows.Skip(state.Page * state.PageSize).Take(state.PageSize).ToList();
        // Return the data
        return Task.FromResult(new TableData<KeyValuePair<uint, Dictionary<string, object>>>() { TotalItems = rows.Length, Items = [] });
    }
}