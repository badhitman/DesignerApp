////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Таблица данных
/// </summary>
public class TableDataModel(IEnumerable<TableDataColumnModel> columns)
{
    /// <summary>
    /// Головна часть таблицы
    /// </summary>
    public TableDataHeadModel Head { get; private set; } = new TableDataHeadModel() { Columns = columns };

    /// <summary>
    /// Тело таблицы
    /// </summary>
    public TableDataBodyModel Body { get; private set; } = new TableDataBodyModel();

    /// <summary>
    /// Добавить строку данных в таблицу
    /// </summary>
    /// <param name="row">Строка данных таблицы</param>
    public void AddRow(TableDataRowModel row)
    {
        Body.Rows.Add(row);
    }
}