////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос сессий (с пагинацией)
/// </summary>
public class RequestSessionsDocumentsRequestPaginationModel : SimplePaginationRequestModel
{
    /// <summary>
    /// Фильтр по схеме документа
    /// </summary>
    public int DocumentSchemeId { get; set; }

    /// <summary>
    /// Фильтр по проекту
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Фильтр автору (user Identity id)
    /// </summary>
    public string? FilterUserId { get; set; }
}