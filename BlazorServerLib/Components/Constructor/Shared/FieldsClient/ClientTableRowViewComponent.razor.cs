////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib.Components;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.FieldsClient;

/// <summary>
/// Client table row view
/// </summary>
public partial class ClientTableRowViewComponent : ComponentBase, IDomBaseComponent
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required IEnumerable<ValueDataForSessionOfDocumentModelDB> RowData { get; set; }

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
    public required TabJoinDocumentSchemeConstructorModelDB PageJoinForm { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required SessionOfDocumentDataModelDB SessionDocument { get; set; }

    /// <inheritdoc/>
    public string DomID => $"tr-{PageJoinForm.Id}-{RowNum}";

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (PageJoinForm.Form is null)
            throw new Exception("данные формы не загружены. error 4551A862-30F4-49BB-85AF-9B7BCEE85EE9");
    }

    string? CalculateFieldValue(FieldFormBaseConstructorModel _fb)
    {
        if (InUse != true)
            return "<калькуляция>";
        else if (string.IsNullOrWhiteSpace(_fb.MetadataValueType) || !CellsValuesOfCurrentRow.Any() || Query(_fb.MetadataValueType) is null)
            return null;

        CommandAsEntryModel? _md = DeclarationAbstraction.ParseCommandsAsEntries(_fb.MetadataValueType);

        if (_md?.Options.Any() != true)
            return null;

        if (DeclarationAbstraction.GetHandlerService(_md.CommandName) is not VirtualColumnCalculationAbstraction _calculate_service || SessionDocument is null || PageJoinForm is null)
            return null;

        Dictionary<string, double> columns = [];
        foreach (string _ff in FieldsNames(_fb.Name))
            if (double.TryParse(CellsValuesOfCurrentRow.FirstOrDefault(x => x.Name.Equals(_ff))?.Value, out double _d))
                columns.Add(_ff, _d);

        if (columns.Count == 0)
            return null;

        return _calculate_service.Calculate(columns, _md.Options).ToString();
    }

    MarkupString GetValue(FieldFormBaseLowConstructorModel _fbl)
    {
        if (_fbl is FieldFormConstructorModelDB _fb && _fb.TypeField == TypesFieldsFormsEnum.ProgramCalculationDouble)
            return (MarkupString)(CalculateFieldValue(_fb) ?? "&nbsp;");

        ValueDataForSessionOfDocumentModelDB? _sv = SessionDocument
        .DataSessionValues?
        .FirstOrDefault(x => x.TabJoinDocumentSchemeId == PageJoinForm.Id && x.RowNum == RowNum && x.Name.Equals(_fbl.Name, StringComparison.OrdinalIgnoreCase));

        return (MarkupString)(_sv?.Value ?? "&nbsp;");
    }

    IQueryable<FieldFormConstructorModelDB>? Query(string field_name) => PageJoinForm.Form!.QueryFieldsOfNumericTypes(field_name);
    IEnumerable<string> FieldsNames(string field_name) => Query(field_name)?.Select(x => x.Name) ?? Enumerable.Empty<string>();
    IEnumerable<ValueDataForSessionOfDocumentModelDB> CellsValuesOfCurrentRow => SessionDocument?.RowsData(PageJoinForm.Id)?.FirstOrDefault(x => x.Key == RowNum) ?? Enumerable.Empty<ValueDataForSessionOfDocumentModelDB>();

    string CellId(FieldFormBaseLowConstructorModel fb) => $"cell-{fb.Id}:{DomID}";

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