////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Простой запрос (с пагинацией)
/// </summary>
public class SimplePaginationRequestModel : PaginationRequestModel
{
    /// <summary>
    /// Строка запроса
    /// </summary>
    public string? SimpleRequest { get; set; }

    /// <inheritdoc/>
    public static SimplePaginationRequestModel Build(string? searchString, int pageSize, int page)
    {
        return new()
        {
            SimpleRequest = searchString,
            PageSize = pageSize,
            PageNum = page,
        };
    }
}