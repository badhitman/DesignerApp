////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запрос заказов (по заявкам)
/// </summary>
public class OrdersByIssuesSelectRequestModel
{
    /// <summary>
    /// Загрузить дополнительные данные для заказов
    /// </summary>
    public bool IncludeExternalData { get; set; }

    /// <summary>
    /// Заказы для заявки из СДЭК
    /// </summary>
    public required int[] IssueIds { get; set; }
}