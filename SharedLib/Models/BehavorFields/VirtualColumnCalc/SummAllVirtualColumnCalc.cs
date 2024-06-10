namespace SharedLib;

/// <summary>
/// Расчёт суммы всех переданных полей (второй параметр: string[] calc_cells) между собой
/// </summary>
public class SummAllVirtualColumnCalc : VirtualColumnCalculationAbstraction
{
    /// <inheritdoc/>
    public override string About => "Сумма полей по их именам. В параметрах указываются имена колонок. Порядок имён не имеет значения";

    /// <inheritdoc/>
    public override string Name => "Сумма";

    /// <inheritdoc/>
    public override double Calculate(Dictionary<string, double> row_cells_data, IEnumerable<string> calc_cells)
    {
        double res = row_cells_data[calc_cells.First()];
        foreach (string cv in calc_cells.Skip(1))
            res += row_cells_data[row_cells_data.Keys.First(x => x.Equals(cv, StringComparison.OrdinalIgnoreCase))];

        return res;
    }
}