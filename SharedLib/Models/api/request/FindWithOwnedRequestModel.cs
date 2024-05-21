////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос поиска с указанием владельца
/// </summary>
public class FindWithOwnedRequestModel : FindRequestModel
{
    /// <summary>
    /// Идентификатор владельца
    /// </summary>
    public string? OwnerId { get; set; }
}