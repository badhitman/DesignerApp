////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Тело таблицы
/// </summary>
public class TableDataBodyModel
{
    /// <summary>
    /// Строки таблицы
    /// </summary>
    public List<TableDataRowModel> Rows { get; set; } = new List<TableDataRowModel>();
}