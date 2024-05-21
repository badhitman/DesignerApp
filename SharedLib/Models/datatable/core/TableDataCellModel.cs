////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Модель ячейки строки таблицы
/// </summary>
public class TableDataCellModel
{
    /// <summary>
    /// Значение
    /// </summary>
    public object? DataCellValue { get; set; }

    /// <summary>
    /// Служебное поле без особого смысла
    /// </summary>
    public string? Tag { get; set; }
}