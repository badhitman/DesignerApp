////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Настройка калькуляции виртуальной колонки таблицы (в группировке)
/// </summary>
public class VirtualColumnCalculateGroupingTableModel
{
    /// <summary>
    /// Имя колонки таблицы
    /// </summary>
    public string ColumnName { get; set; } = default!;

    /// <summary>
    /// Идентификатор связи формы со страницей опроса/анкеты
    /// </summary>
    public int JoinFormId { get; set; }

    /// <summary>
    /// Имя типа данных калькулятора (реализация от VirtualColumnCalc)
    /// </summary>
    public string CalculationName { get; set; } = default!;
    
    /// <summary>
    /// Параметры вызова калькулятора
    /// </summary>
    public string[] FieldsNames { get; set; } = default!;
}