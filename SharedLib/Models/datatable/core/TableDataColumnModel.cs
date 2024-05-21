////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Модель заголовка колонки таблицы
/// </summary>
public class TableDataColumnModel
{
    /// <summary>
    /// Заголовок колонки таблицы
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Имя поля данных (системное/sql)
    /// </summary>
    public required string ColumnDataName { get; set; }

    /// <summary>
    /// Направление сортировки таблицы
    /// </summary>
    public VerticalDirectionsEnum? SortingDirection { get; set; } = null;

    /// <summary>
    /// Стиль колонки (css/style)
    /// </summary>
    public string? Style { get; set; }
}