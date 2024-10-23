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
    /// <remarks>
    /// .Include(x => x.Organization)
    /// .Include(x => x.AddressesTabs)
    /// .ThenInclude(x => x.Rows)
    /// .ThenInclude(x => x.Offer)
    /// .ThenInclude(x => x.Goods)
    /// </remarks>
    public bool IncludeExternalData { get; set; }
    
    /// <summary>
    /// Заказы для заявки из СДЭК
    /// </summary>
    public required int[] IssueIds { get; set; }
}