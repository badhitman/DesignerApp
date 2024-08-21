////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос с пагинацией
/// </summary>
public class TPaginationRequestModel<T> : PaginationRequestModel
{
    /// <summary>
    /// Payload
    /// </summary>
    public required T Payload { get; set; }
}
