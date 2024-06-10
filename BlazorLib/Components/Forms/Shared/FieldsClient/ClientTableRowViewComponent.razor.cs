using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsClient;

public partial class ClientTableRowViewComponent : ComponentBase
{
    [Inject]
    protected ILogger<ClientTableRowViewComponent> _logger { get; set; } = default!;

    [Parameter, EditorRequired]
    public IEnumerable<ConstructorFormSessionValueModelDB> RowData { get; set; } = default!;

    [Parameter, EditorRequired]
    public Action<uint> DeleteHandle { get; set; } = default!;

    [Parameter, EditorRequired]
    public Action<uint> OpenHandle { get; set; } = default!;

    [Parameter, EditorRequired]
    public uint RowNum { get; set; }

    [CascadingParameter]
    public bool? InUse { get; set; }

    [CascadingParameter, EditorRequired]
    public ConstructorFormQuestionnairePageJoinFormModelDB PageJoinForm { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public ConstructorFormSessionModelDB SessionQuestionnaire { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public ConstructorFormModelDB Form { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    IEnumerable<EntryAltDescriptionModel> Entries { get; set; } = default!;

    [CascadingParameter]
    public ConstructorFormQuestionnairePageModelDB? QuestionnairePage { get; set; }

    string? CalcFieldValue(ConstructorFieldFormBaseModel _fb)
    {
        if (InUse != true)
            return "<калькуляция>";
        else if (string.IsNullOrWhiteSpace(_fb.MetadataValueType) || !CellsValuesOfCurrentRow.Any() || query(_fb.MetadataValueType) is null)
            return null;

        CommandsAsEntriesModel? _md = DeclarationAbstraction.ParseCommandsAsEntries(_fb.MetadataValueType);

        if (_md?.Options.Any() != true)
            return null;

        if (VirtualColumnCalculationAbstraction.GetHandlerService(_md.CommandName) is not VirtualColumnCalculationAbstraction _calc_s || SessionQuestionnaire is null || PageJoinForm is null)
            return null;

        Dictionary<string, double> columns = [];
        foreach (string _ff in fields_names(_fb.Name))
            if (double.TryParse(CellsValuesOfCurrentRow.FirstOrDefault(x => x.Name.Equals(_ff))?.Value, out double _d))
                columns.Add(_ff, _d);

        if (columns.Count == 0)
            return null;

        return _calc_s.Calculate(columns, _md.Options).ToString();
    }

    MarkupString GetValue(ConstructorFieldFormBaseLowModel _fbl)
    {
        if (_fbl is ConstructorFieldFormModelDB _fb && _fb.TypeField == TypesFieldsFormsEnum.ProgrammCalcDouble)
            return (MarkupString)(CalcFieldValue(_fb) ?? "&nbsp;");

        ConstructorFormSessionValueModelDB? _sv = SessionQuestionnaire
        .SessionValues?
        .FirstOrDefault(x => x.QuestionnairePageJoinFormId == PageJoinForm.Id && x.GroupByRowNum == RowNum && x.Name.Equals(_fbl.Name, StringComparison.OrdinalIgnoreCase));

        return (MarkupString)(_sv?.Value ?? "&nbsp;");
    }

    IQueryable<ConstructorFieldFormModelDB>? query(string field_name) => PageJoinForm?.Form?.QueryFieldsOfNumericTypes(field_name);
    IEnumerable<string> fields_names(string field_name) => query(field_name)?.Select(x => x.Name) ?? Enumerable.Empty<string>();
    IEnumerable<ConstructorFormSessionValueModelDB> CellsValuesOfCurrentRow => SessionQuestionnaire?.RowsData(PageJoinForm!.Id)?.FirstOrDefault(x => x.Key == RowNum) ?? Enumerable.Empty<ConstructorFormSessionValueModelDB>();

    string CellId(ConstructorFieldFormBaseLowModel fb) => $"cell-{PageJoinForm.Id}-{RowNum}_{fb.Id}";

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

    void DeletAction() => DeleteHandle(RowNum);

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }
}