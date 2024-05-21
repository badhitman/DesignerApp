////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Головная часть таблицы
/// </summary>
public class TableDataHeadModel
{
    /// <summary>
    /// Колонки таблицы
    /// </summary>
    public required IEnumerable<TableDataColumnModel> Columns { get; set; }
}