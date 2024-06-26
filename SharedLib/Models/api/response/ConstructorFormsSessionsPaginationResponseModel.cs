////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Сессии опросов/анкет
/// </summary>
public class ConstructorFormsSessionsPaginationResponseModel : PaginationResponseModel
{
    /// <summary>
    /// Сессии опросов/анкет
    /// </summary>
    public ConstructorFormsSessionsPaginationResponseModel() { }

    /// <summary>
    /// Сессии опросов/анкет
    /// </summary>
    public ConstructorFormsSessionsPaginationResponseModel(PaginationRequestModel req) { }

    /// <summary>
    /// Сессии опросов/анкет
    /// </summary>
    public List<SessionOfDocumentDataModelDB>? Sessions { get; set; }
}