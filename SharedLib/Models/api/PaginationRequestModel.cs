////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Базовая модель запроса с поддержкой пагинации
/// </summary>
public class PaginationRequestModel
{
    /// <inheritdoc/>
    public static PaginationRequestModel Build(PaginationRequestModel init_object)
    {
        return new()
        {
            PageSize = init_object.PageSize,
            PageNum = init_object.PageNum,
            SortingDirection = init_object.SortingDirection,
            SortBy = init_object.SortBy
        };
    }

    /// <summary>
    /// Размер страницы (количество элементов на странице)
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Номер текущей страницы
    /// </summary>
    public int PageNum { get; set; }

    /// <summary>
    /// Сортировка (от большего к меньшему или от меньшего к большему)
    /// </summary>
    public VerticalDirectionsEnum SortingDirection { get; set; }

    /// <summary>
    /// Имя поля по которому должна происходить сортировка
    /// </summary>
    public string? SortBy { get; set; }
}