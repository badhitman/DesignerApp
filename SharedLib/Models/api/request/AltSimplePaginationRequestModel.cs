namespace SharedLib;

/// <summary>
/// Простой запрос (с пагинацией)
/// </summary>
public class AltSimplePaginationRequestModel : SimplePaginationRequestModel
{
    /// <summary>
    /// Режим строгой проверки
    /// </summary>
    public bool StrongMode { get; set; }
}