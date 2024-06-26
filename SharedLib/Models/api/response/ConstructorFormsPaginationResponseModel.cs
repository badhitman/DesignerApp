////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Формы (с пагинацией)
/// </summary>
public class ConstructorFormsPaginationResponseModel : PaginationResponseModel
{
    /// <summary>
    /// Формы (с пагинацией)
    /// </summary>
    public ConstructorFormsPaginationResponseModel() { }

    /// <summary>
    /// Формы (с пагинацией)
    /// </summary>
    public ConstructorFormsPaginationResponseModel(PaginationRequestModel req)  { }

    /// <summary>
    /// Формы
    /// </summary>
    public IEnumerable<FormConstructorModelDB>? Elements { get; set; }
}