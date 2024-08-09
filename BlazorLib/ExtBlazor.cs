using SharedLib;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace BlazorLib;

/// <summary>
/// ExtBlazor
/// </summary>
public static class ExtBlazor
{
    /// <summary>
    /// Название параметра в URL
    /// </summary>
    public const string ActiveTabName = "TabName";

    /// <summary>
    /// Получит имя вкладки
    /// </summary>
    public static string? GetTabNameFromUrl(this NavigationManager nav)
    {
        Uri uri = new(nav.Uri);
        Dictionary<string, Microsoft.Extensions.Primitives.StringValues> queryParameters = QueryHelpers.ParseQuery(uri.Query);

        return queryParameters.TryGetValue(ActiveTabName, out var tabName)
            ? tabName.ToString()
            : null;
    }

    /// <inheritdoc/>
    public static void Error(this ISnackbar SnackbarRepo, string message)
        => SnackbarRepo.Add(message, Severity.Error, opt => opt.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

    /// <inheritdoc/>
    public static VerticalDirectionsEnum GetVerticalDirection(this SortDirection sort_direction)
    {
        return sort_direction switch
        {
            SortDirection.Descending => VerticalDirectionsEnum.Down,
            SortDirection.Ascending => VerticalDirectionsEnum.Up,
            _ => VerticalDirectionsEnum.Up
        };
    }
}