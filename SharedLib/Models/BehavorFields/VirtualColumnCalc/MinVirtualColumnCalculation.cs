﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Вычисление минимального значения из полей (второй параметр: string[] calk_cells)
/// </summary>
public class MinVirtualColumnCalculation : VirtualColumnCalculationAbstraction
{
    /// <inheritdoc/>
    public override string About => "Вычисление минимального значения из полей по их именам. В параметрах указываются имена колонок (если имена не указать, то для всех числовых полей). Порядок имён не имеет значения";

    /// <inheritdoc/>
    public override string Name => "Min";

    /// <inheritdoc/>
    public override double Calculate(Dictionary<string, double> row_cells_data, IEnumerable<string>? calk_cells)
    {
        if (calk_cells?.Any(x => !row_cells_data.Any(y => x.Equals(y.Key))) == true)
            return 0;

        calk_cells ??= row_cells_data.Select(x => x.Key).ToList();

        IOrderedEnumerable<string> oq = calk_cells.Order();
        double res = row_cells_data[oq.First()];
        foreach (string cv in oq.Skip(1))
            res = double.Min(res, row_cells_data[row_cells_data.Keys.First(x => x.Equals(cv, StringComparison.OrdinalIgnoreCase))]);

        return res;
    }
}
