////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Базовая модель ответа с поддержкой пагинации
/// </summary>
public class PaginationResponseModel : PaginationRequestModel
{
    /// <summary>
    /// Общее/всего количество элементов
    /// </summary>
    public int TotalRowsCount { get; set; }

    /// <summary>
    /// Количество страниц пагинатора
    /// </summary>
    /// <param name="page_size"></param>
    /// <param name="total_rows_count"></param>
    /// <param name="default_page_size"></param>
    /// <returns></returns>
    public static uint CalcTotalPagesCount(int page_size,int total_rows_count, uint default_page_size = 10)
    {
        if (page_size == 0)
            return (uint)Math.Ceiling((double)total_rows_count / (double)default_page_size);

        return (uint)Math.Ceiling((double)total_rows_count / (double)page_size);
    }
}