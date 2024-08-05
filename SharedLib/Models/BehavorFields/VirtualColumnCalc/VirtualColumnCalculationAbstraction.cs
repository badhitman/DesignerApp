////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Виртуальная колонка
/// </summary>
public abstract class VirtualColumnCalculationAbstraction : DeclarationAbstraction
{
    /// <summary>
    /// Рассчитать результат
    /// </summary>
    /// <param name="row_cells_data">Данные строки (все колонки/поля данных)</param>
    /// <param name="calculation_cells">поля для расчётов</param>
    /// <returns>Результат выполнения функции</returns>
    public abstract double Calculate(Dictionary<string, double> row_cells_data, IEnumerable<string>? calculation_cells);
}