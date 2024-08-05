////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос поиска
/// </summary>
public class FindRequestModel : PaginationRequestModel
{
    /// <summary>
    /// Строка поиска
    /// </summary>
    public required string FindQuery { get; set; }
}