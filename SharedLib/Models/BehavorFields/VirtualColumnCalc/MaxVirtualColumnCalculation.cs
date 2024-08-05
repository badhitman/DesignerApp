////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Вычисление максимального значения из полей (второй параметр: string[] calk_cells)
/// </summary>
public class MaxVirtualColumnCalculation : VirtualColumnCalculationAbstraction
{
    /// <inheritdoc/>
    public override string About => "Вычисление максимального значения из полей по их именам. В параметрах указываются имена колонок (если имена не указать, то для всех числовых полей). Порядок имён не имеет значения";

    /// <inheritdoc/>
    public override string Name => "Max";

    /// <inheritdoc/>
    public override double Calculate(Dictionary<string, double> row_cells_data, IEnumerable<string>? calk_cells)
    {
        if (calk_cells?.Any(x => !row_cells_data.Any(y => x.Equals(y.Key))) == true)
            return 0;

        calk_cells ??= row_cells_data.Select(x => x.Key).ToList();

        double res = row_cells_data[calk_cells.First()];
        foreach (string cv in calk_cells.Skip(1))
            res = double.Max(res, row_cells_data[row_cells_data.Keys.First(x => x.Equals(cv, StringComparison.OrdinalIgnoreCase))]);

        return res;
    }
}
