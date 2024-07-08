////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Расчёт суммы всех переданных полей (второй параметр: string[] calk_cells) между собой
/// </summary>
public class SumAllVirtualColumnCalculation : VirtualColumnCalculationAbstraction
{
    /// <inheritdoc/>
    public override string About => "Сумма полей по их именам. В параметрах указываются имена колонок (если имена не указать, то для всех числовых полей). Порядок имён не имеет значения";

    /// <inheritdoc/>
    public override string Name => "Сумма";

    /// <inheritdoc/>
    public override double Calculate(Dictionary<string, double> row_cells_data, IEnumerable<string>? calk_cells)
    {
        if (calk_cells?.Any(x => !row_cells_data.Any(y => x.Equals(y.Key))) == true)
            return 0;

        calk_cells ??= row_cells_data.Select(x => x.Key).ToList();

        double res = row_cells_data[calk_cells.First()];
        foreach (string cv in calk_cells.Skip(1))
            res += row_cells_data[row_cells_data.Keys.First(x => x.Equals(cv, StringComparison.OrdinalIgnoreCase))];

        return res;
    }
}