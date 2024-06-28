////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Управление таблицей группировок/сумм
/// </summary>
public class FormTableCalculationManager
{
    SelectedFieldModel _selected_field = default!;
    readonly Dictionary<string, List<Dictionary<string, double>>> _data = [];
    IQueryable<FieldFormConstructorModelDB>? Query => _page_join_form.Form?.QueryFieldsOfNumericTypes(_selected_field.FieldName ?? "");

    TabJoinDocumentSchemeConstructorModelDB _page_join_form = default!;

    /// <summary>
    /// Ведущая колонка (выбранное поле)
    /// </summary>
    public IEnumerable<(string MainGroupingValue, int CountRows)> MainCol => [.. _data
                .Select(x => (MainGroupingValue: x.Key, CountRows: x.Value.Count()))
                .OrderBy(x => x.MainGroupingValue)];

    /// <summary>
    /// Имя выбранного поля
    /// </summary>
    public string FieldName => _selected_field.FieldName;

    /// <summary>
    /// Управление таблицей группировок/сумм
    /// </summary>
    public FormTableCalculationManager(SelectedFieldModel selected_field, TabJoinDocumentSchemeConstructorModelDB page_join_form, SessionOfDocumentDataModelDB session_questionnairie)
        => Update(selected_field, page_join_form, session_questionnairie);

    /// <summary>
    /// Получить строку данных по мастер-значению
    /// </summary>
    /// <exception cref="InvalidOperationException">Данные не найдены по ключу мастер-значения</exception>
    public Dictionary<string, double> GetRow(string row_master_value)
    {
        if (!_data.TryGetValue(row_master_value, out List<Dictionary<string, double>>? row_data) || row_data is null)
            throw new InvalidOperationException($"Данные не найдены по ключу мастер-значения '{row_master_value}'. error 9E75DE61-1BCD-42D2-957F-B6A871A1A559");
        Dictionary<string, double> res = [];
        foreach (KeyValuePair<string, double> _kvp in row_data.SelectMany(x => x))
        {
            if (res.ContainsKey(_kvp.Key))
                res[_kvp.Key] += _kvp.Value;
            else
                res.Add(_kvp.Key, _kvp.Value);
        }

        return res;
    }

    /// <summary>
    /// Сумма общая по колонке
    /// </summary>
    public double GetTotalSumFieldForGroup(string g_field_name)
    {
        double res = 0;
        foreach (KeyValuePair<string, List<Dictionary<string, double>>> _d in _data)
            foreach (Dictionary<string, double> _r in _d.Value)
                if (_r.TryGetValue(g_field_name, out double _v))
                    res += _v;

        return res;
    }

    /// <inheritdoc/>
    public double CalcVirtualColumn(VirtualColumnCalculateGroupingTableModel vcg, string row_master_value)
    {
        if (DeclarationAbstraction.GetHandlerService(vcg.CalculationName) is not VirtualColumnCalculationAbstraction _calc_s)
            return 0;

        Dictionary<string, double> columns = [];
        foreach (KeyValuePair<string, double> _kvp in GetRow(row_master_value).Where(x => vcg.FieldsNames.Contains(x.Key)))
            if (columns.ContainsKey(_kvp.Key))
                columns[_kvp.Key] += _kvp.Value;
            else
                columns.Add(_kvp.Key, _kvp.Value);

        return _calc_s.Calculate(columns, vcg.FieldsNames);
    }

    /// <summary>
    /// 
    /// </summary>
    public double GetTotalSumVirtualColumnForGroup(VirtualColumnCalculateGroupingTableModel vcg)
    {
        double total_sum = 0;
        foreach ((string MainGroupingValue, int CountRows) _col in MainCol)
            total_sum += CalcVirtualColumn(vcg, _col.MainGroupingValue);

        return total_sum;
    }

    /// <summary>
    /// Обновить объект
    /// </summary>
    public void Update(SelectedFieldModel selected_field, TabJoinDocumentSchemeConstructorModelDB page_join_form, SessionOfDocumentDataModelDB session_questionnairie)
    {
        _selected_field = selected_field;
        _page_join_form = page_join_form;
        _data.Clear();

        foreach (IGrouping<uint, ValueDataForSessionOfDocumentModelDB> sv in session_questionnairie.RowsData(page_join_form.Id)!)
        {
            ValueDataForSessionOfDocumentModelDB? master_sv = sv.FirstOrDefault(x => x.Name.Equals(_selected_field.FieldName))
                ?? ValueDataForSessionOfDocumentModelDB.Build(SetValueFieldDocumentDataModel.Build("", selected_field.FieldName, "<null>", sv.Key), page_join_form, session_questionnairie);

            SetData(sv.Where(x => !x.Name.Equals(_selected_field.FieldName) && Query?.Any(y => y.Name.Equals(x.Name)) == true).ToArray(), master_sv);
        }
    }

    void SetData(IEnumerable<ValueDataForSessionOfDocumentModelDB> values, ValueDataForSessionOfDocumentModelDB master_sv)
    {
        string _pos = master_sv.Value ?? "";
        if (!_data.ContainsKey(_pos))
            _data.Add(_pos, new());

        Dictionary<string, double> row = new();
        foreach (ValueDataForSessionOfDocumentModelDB val in values)
        {
            if (string.IsNullOrWhiteSpace(val.Value))
                row.Add(val.Name, 0.0);
            else if (double.TryParse(val.Value, out double dv))
                row.Add(val.Name, dv);
            else
                throw new Exception($"Не корректное значение '{val.Value}'. error 9BBBD859-5AF4-469C-863F-D99BA6FE6E6C");
        }
        _data[_pos].Add(row);
    }
}