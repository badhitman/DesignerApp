namespace SharedLib;

/// <summary>
/// Вычисление требуемого количества нод
/// </summary>
public class NodesCount8x16VirtualColumnCalc : VirtualColumnCalculationAbstraction
{
    /// <inheritdoc/>
    public override string About => "Вычисление требуемого количества нод кратно (cpu:8 && ram:16). Входящие имена полей должны иметь следующий порядок: количество_CPU, количество_RAM, количество_реплик. Имена колонок могут быть любыми, но их порядок обязательно такой.";

    /// <inheritdoc/>
    public override string Name => "Кол-во нод (C8 R16)";

    /// <inheritdoc/>
    public override double Calculate(Dictionary<string, double> row_cells_data, IEnumerable<string> calc_cells)
    {
        if (calc_cells.Count() != 3 || calc_cells.Any(x => !row_cells_data.Any(y => x.Equals(y.Key))))
            return 0;

        int repl = (int)row_cells_data[calc_cells.Skip(2).First()];
        int cpu = repl * (int)Math.Ceiling(row_cells_data[calc_cells.First()] / 8);
        int ram = repl * ((int)Math.Ceiling(row_cells_data[calc_cells.Skip(1).First()] / 16));

        return Math.Max(cpu, ram);
    }
}