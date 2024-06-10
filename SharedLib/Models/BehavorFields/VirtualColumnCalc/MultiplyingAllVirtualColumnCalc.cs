namespace SharedLib;

/// <summary>
/// Расчёт умножения всех переданных полей (второй параметр: string[] calc_cells) между собой
/// </summary>
public class MultiplyingAllVirtualColumnCalc : VirtualColumnCalculationAbstraction
{
    /// <inheritdoc/>
    public override string About => "Умножения полей между собой по их именам. В параметрах указываются имена колонок. Порядок имён не имеет значения";

    /// <inheritdoc/>
    public override string Name => "Умножение";

    /// <inheritdoc/>
    public override double Calculate(Dictionary<string, double> row_cells_data, IEnumerable<string> calc_cells)
    {
        if (calc_cells.Any(x => !row_cells_data.Any(y => x.Equals(y.Key))))
            return 0;

        double res = row_cells_data[calc_cells.First()];
        foreach (string cv in calc_cells.Skip(1))
            res *= row_cells_data[row_cells_data.Keys.First(x => x.Equals(cv, StringComparison.OrdinalIgnoreCase))];

        return res;
    }
}
