////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// Организации пользователя
/// </summary>
public partial class OrganizationsForUserPage : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Пользователь, для которого отобразить организации
    /// </summary>
    [Parameter]
    public string? UserId { get; set; }


    private IEnumerable<OrganizationModelDB> pagedData = default!;
    private MudTable<OrganizationModelDB> table = default!;

    private int totalItems;

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<OrganizationModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        //IEnumerable<OrganizationModelDB> data = await httpClient.GetFromJsonAsync<List<OrganizationModelDB>>("webapi/periodictable", token);
        //await Task.Delay(300, token);
        //data = data.Where(element =>
        //{
        //    if (string.IsNullOrWhiteSpace(searchString))
        //        return true;
        //    if (element.Sign.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        //        return true;
        //    if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        //        return true;
        //    if ($"{element.Number} {element.Position} {element.Molar}".Contains(searchString))
        //        return true;
        //    return false;
        //}).ToArray();
        //totalItems = data.Count();
        //switch (state.SortLabel)
        //{
        //    case "nr_field":
        //        data = data.OrderByDirection(state.SortDirection, o => o.Number);
        //        break;
        //    case "sign_field":
        //        data = data.OrderByDirection(state.SortDirection, o => o.Sign);
        //        break;
        //    case "name_field":
        //        data = data.OrderByDirection(state.SortDirection, o => o.Name);
        //        break;
        //    case "position_field":
        //        data = data.OrderByDirection(state.SortDirection, o => o.Position);
        //        break;
        //    case "mass_field":
        //        data = data.OrderByDirection(state.SortDirection, o => o.Molar);
        //        break;
        //}

        //pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        return new TableData<OrganizationModelDB>() { TotalItems = totalItems, Items = pagedData };
    }
}