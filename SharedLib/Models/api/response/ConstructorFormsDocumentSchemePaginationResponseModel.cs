////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Схемы документов (с пагинацией)
/// </summary>
public class ConstructorFormsDocumentSchemePaginationResponseModel : PaginationResponseModel
{
    /// <summary>
    /// Схемы документов (с пагинацией)
    /// </summary>
    public ConstructorFormsDocumentSchemePaginationResponseModel() { }

    /// <summary>
    /// Схемы документов (с пагинацией)
    /// </summary>
    public ConstructorFormsDocumentSchemePaginationResponseModel(PaginationRequestModel req) { }

    /// <summary>
    /// Схемы документов
    /// </summary>
    public IEnumerable<DocumentSchemeConstructorModelDB>? DocumentsSchemes { get; set; }
}