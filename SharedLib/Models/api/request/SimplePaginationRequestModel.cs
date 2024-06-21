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
}