////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
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
    [CascadingParameter, EditorRequired]
    public required List<ValueDataForSessionOfDocumentModelDB> SessionValues { get; set; }

    /// <summary>
    /// Вкладка/таб
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabJoinDocumentSchemeConstructorModelDB Join { get; set; }

    /// <summary>
    /// Таблица редактируется через свои собственные посадочные стрраницы формы
    /// </summary>
    public override bool IsEdited => false;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server, with a token for canceling this request
    /// </summary>
    private Task<TableData<KeyValuePair<int, Dictionary<string, object>>>> ServerReload(TableState state, CancellationToken token)
    {
        // Forward the provided token to methods which support it
        //var data = await httpClient.GetFromJsonAsync<List<Dictionary<int, Dictionary<string,object>>>>("webapi/periodictable", token);
        // Simulate a long-running operation
        //await Task.Delay(300, token);
        // Get the total count
        //var totalItems = data.Count();
        // Get the paged data
        //var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToList();
        // Return the data
        return Task.FromResult(new TableData<KeyValuePair<int, Dictionary<string, object>>>() { TotalItems = 0, Items = [] });
    }
}