namespace SharedLib;

/// <summary>
/// Виртуальная колонка
/// </summary>
public abstract class VirtualColumnCalcAbstraction : DeclarationAbstraction
{
    /// <summary>
    /// Рассчитать результат
    /// </summary>
    /// <param name="row_cells_data">Данные строки (все колонки/поля данных)</param>
    /// <param name="calc_cells">поля для расчётов</param>
    /// <returns>Резульатт выполнения функции</returns>
    public abstract double Calc(Dictionary<string, double> row_cells_data, IEnumerable<string> calc_cells);
}