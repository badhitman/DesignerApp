﻿using BlazorLib.Components;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.FieldsClient;

/// <summary>
/// Client table row view
/// </summary>
public partial class ClientTableRowViewComponent : ComponentBase, IDomBaseComponent
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required IEnumerable<ConstructorFormSessionValueModelDB> RowData { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required Action<uint> DeleteHandle { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required Action<uint> OpenHandle { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required uint RowNum { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public bool? InUse { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormQuestionnairePageJoinFormModelDB PageJoinForm { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormSessionModelDB SessionQuestionnaire { get; set; }

    /// <inheritdoc/>
    public string DomID => $"tr-{PageJoinForm.Id}-{RowNum}";

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (PageJoinForm.Form is null)
            throw new Exception("данные формы не загружены. error 4551A862-30F4-49BB-85AF-9B7BCEE85EE9");
    }

    string? CalculateFieldValue(ConstructorFieldFormBaseModel _fb)
    {
        if (InUse != true)
            return "<калькуляция>";
        else if (string.IsNullOrWhiteSpace(_fb.MetadataValueType) || !CellsValuesOfCurrentRow.Any() || Query(_fb.MetadataValueType) is null)
            return null;

        CommandsAsEntriesModel? _md = DeclarationAbstraction.ParseCommandsAsEntries(_fb.MetadataValueType);

        if (_md?.Options.Any() != true)
            return null;

        if (DeclarationAbstraction.GetHandlerService(_md.CommandName) is not VirtualColumnCalculationAbstraction _calculate_service || SessionQuestionnaire is null || PageJoinForm is null)
            return null;

        Dictionary<string, double> columns = [];
        foreach (string _ff in FieldsNames(_fb.Name))
            if (double.TryParse(CellsValuesOfCurrentRow.FirstOrDefault(x => x.Name.Equals(_ff))?.Value, out double _d))
                columns.Add(_ff, _d);

        if (columns.Count == 0)
            return null;

        return _calculate_service.Calculate(columns, _md.Options).ToString();
    }

    MarkupString GetValue(ConstructorFieldFormBaseLowModel _fbl)
    {
        if (_fbl is ConstructorFieldFormModelDB _fb && _fb.TypeField == TypesFieldsFormsEnum.ProgrammCalcDouble)
            return (MarkupString)(CalculateFieldValue(_fb) ?? "&nbsp;");

        ConstructorFormSessionValueModelDB? _sv = SessionQuestionnaire
        .SessionValues?
        .FirstOrDefault(x => x.QuestionnairePageJoinFormId == PageJoinForm.Id && x.GroupByRowNum == RowNum && x.Name.Equals(_fbl.Name, StringComparison.OrdinalIgnoreCase));

        return (MarkupString)(_sv?.Value ?? "&nbsp;");
    }

    IQueryable<ConstructorFieldFormModelDB>? Query(string field_name) => PageJoinForm.Form!.QueryFieldsOfNumericTypes(field_name);
    IEnumerable<string> FieldsNames(string field_name) => Query(field_name)?.Select(x => x.Name) ?? Enumerable.Empty<string>();
    IEnumerable<ConstructorFormSessionValueModelDB> CellsValuesOfCurrentRow => SessionQuestionnaire?.RowsData(PageJoinForm.Id)?.FirstOrDefault(x => x.Key == RowNum) ?? Enumerable.Empty<ConstructorFormSessionValueModelDB>();

    string CellId(ConstructorFieldFormBaseLowModel fb) => $"cell-{fb.Id}:{DomID}";

    int _total_cols_count = -1;
    int TotalColsCount
    {
        get
        {
            if (_total_cols_count < 0)
                _total_cols_count = PageJoinForm.Form!.AllFields.Count();

            return _total_cols_count;
        }
    }

    void OpenEditAction() => OpenHandle(RowNum);

    void DeleteAction() => DeleteHandle(RowNum);
}