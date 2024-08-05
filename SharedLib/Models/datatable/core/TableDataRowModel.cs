////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Модель строки таблицы
/// </summary>
public class TableDataRowModel : IdSwitchableModel
{
    /// <summary>
    /// Ячейки строки таблицы
    /// </summary>
    public required IEnumerable<TableDataCellModel> Cells { get; set; }
}