////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Простой запрос (с пагинацией)
/// </summary>
public class AltSimplePaginationRequestModel : SimplePaginationRequestModel
{
    /// <inheritdoc/>
    public static AltSimplePaginationRequestModel Build(string? simpleRequest, int pageSize = 10, int pageNum = 0, bool strongMode = false)
    {
        return new AltSimplePaginationRequestModel()
        {
            PageNum = pageNum,
            SimpleRequest = simpleRequest,
            PageSize = pageSize,
            StrongMode = strongMode
        };
    }

    /// <summary>
    /// Режим строгой проверки
    /// </summary>
    public bool StrongMode { get; set; }
}