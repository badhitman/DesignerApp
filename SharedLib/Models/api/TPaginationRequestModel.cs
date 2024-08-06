////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос с пагинацией
/// </summary>
public class TPaginationRequestModel<T>: PaginationRequestModel
{
    /// <summary>
    /// 
    /// </summary>
    public required T Request {  get; set; }
}
